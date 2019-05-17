using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace DeptNews.LeaveList
{
    public partial class LeaveListUserControl : UserControl
    {
        public LeaveList WebPartObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            string listName=WebPartObj.ListName;//请假列表名称
            string siteUrl = WebPartObj.SiteUrl;//网址
            string userList = WebPartObj.UserList;//用户列表名
            
            string wpFeature = WebPartObj.WPFeature;
            string approveUrl = WebPartObj.WebUrl;
            ReadItems(listName, siteUrl, userList, wpFeature);
        }

        /// <summary>
        /// 获取当前用户账户（不包含AD）
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public string GetAccount()
        {
            string loginName = SPContext.Current.Web.CurrentUser.LoginName;
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        /// <summary>
        /// 获取用户角色与部门
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="lgName"></param>
        /// <returns></returns>
        private string[] GetUserDuty()
        {
            string[] userInfo = new string[2];
            string listTeachers = WebPartObj.UserList;
            string siteUrl = SPContext.Current.Site.Url;
            SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
            SPListItemCollection retItems = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.RootWeb)
                    {
                        SPQuery qry = new SPQuery();
                        SPListItemCollection listItems;

                        #region teachers
                        SPList spTeacherList = spWeb.Lists.TryGetList(listTeachers);

                        qry = new SPQuery();
                        qry.Query = @"<Where><Eq><FieldRef Name='EmpNO' /><Value Type='Text'>" + GetAccount() + "</Value></Eq></Where>";
                        listItems = spTeacherList.GetItems(qry);
                        if (listItems.Count > 0)//获取系部下的教师
                        {
                            retItems = listItems;
                        }
                        #endregion
                    }
                }
            });
            if (retItems != null)
            {
                userInfo[0] = retItems[0]["Duty"] != null ? retItems[0]["Duty"].ToString() : "普通教师";
                userInfo[1] = retItems[0]["Department"] != null ? retItems[0]["Department"].ToString() : "";                      
            }
            return userInfo;
        }
        /// <summary>
        /// 获取当前用户的身份
        /// 1:普通教师;
        /// 2:系部负责人;
        /// 3:主管教学领导
        /// 4:主管科研领导
        /// 5:主管人事领导
        /// 
        /// </summary>
        /// <param name="userList"></param>
        protected string GetQueryStr(string userList)
        {            
            string queryStr = "";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                SPWeb spWeb = SPContext.Current.Site.RootWeb;
                SPList spList = spWeb.Lists.TryGetList(userList);
                string userName = GetAccount();
                if (spList != null)
                {
                    int roleId = 0;
                    string[] userInfo = GetUserDuty();
                    switch (userInfo[0])//教师身份
                    {
                        case "系部领导":
                            string dept = userInfo[1];
                            roleId = 2;
                            //queryStr = "<Where><And><Eq><FieldRef Name='Dept'/><Value Type='Text'>" + dept + "</Value></Eq><Eq><FieldRef Name ='State'/><Value Type='Text'>系部负责人待审批</Value></Eq></And></Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                            queryStr = @"<Where>
                                          <And>
                                             <And>
                                                <Or>
                                                   <Eq>
                                                      <FieldRef Name='Flag' />
                                                      <Value Type='Number'>0</Value>
                                                   </Eq>
                                                   <IsNull>
                                                       <FieldRef Name='Flag' />
                                                    </IsNull>
                                                </Or>
                                                <Lt>
                                                   <FieldRef Name='ApproveType' />
                                                   <Value Type='Number'>2</Value>
                                                </Lt>
                                             </And>";
                            queryStr += "<Eq>< FieldRef Name = 'Dept'/><Value Type = 'Text'>"+ dept + "</Value></Eq></And></Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                            break;
                        case "分管教学领导":
                            roleId = 3;
                            queryStr = @"<Where>
                                              <And>
                                                 <And>
                                                    <And>
                                                       <Eq>
                                                          <FieldRef Name='Flag' />
                                                          <Value Type='Number'>11</Value>
                                                       </Eq>
                                                       <Gt>
                                                          <FieldRef Name='ApproveType' />
                                                          <Value Type='Number'>0</Value>
                                                       </Gt>
                                                    </And>
                                                    <Lt>
                                                       <FieldRef Name='ApproveType' />
                                                       <Value Type='Number'>3</Value>
                                                    </Lt>
                                                 </And>
                                                 <Neq>
                                                    <FieldRef Name='ReimburseType' />
                                                    <Value Type='Choice'>科研</Value>
                                                 </Neq>
                                              </And>
                                            </Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                            break;
                        case "分管科研领导":
                            roleId = 4;
                            queryStr = @" <Where>
                                              <And>
                                                 <And>
                                                    <And>
                                                       <Eq>
                                                          <FieldRef Name='Flag' />
                                                          <Value Type='Number'>11</Value>
                                                       </Eq>
                                                       <Gt>
                                                          <FieldRef Name='ApproveType' />
                                                          <Value Type='Number'>0</Value>
                                                       </Gt>
                                                    </And>
                                                    <Eq>
                                                       <FieldRef Name='Reimburse' />
                                                       <Value Type='Boolean'>1</Value>
                                                    </Eq>
                                                 </And>
                                                 <Eq>
                                                    <FieldRef Name='ReimburseType' />
                                                    <Value Type='Choice'>科研</Value>
                                                 </Eq>
                                              </And>
                                           </Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                            break;
                        case "人事领导":
                            roleId = 5;
                            queryStr = @"<Where>
                                          <Or>
                                             <And>
                                                <Eq>
                                                   <FieldRef Name='Flag' />
                                                   <Value Type='Number'>11</Value>
                                                </Eq>
                                                <Eq>
                                                   <FieldRef Name='ApproveType' />
                                                   <Value Type='Number'>0</Value>
                                                </Eq>
                                             </And>
                                             <Eq>
                                                <FieldRef Name='Flag' />
                                                <Value Type='Number'>21</Value>
                                             </Eq>
                                          </Or>
                                       </Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                            break;
                        default:
                            roleId = 1;
                            queryStr = "";
                            break;
                    }
                    
                }
            });
            return queryStr;
        }

        public void ReadItems(string listName, string siteUrl, string userList, string wpFeature)
        {
            if (siteUrl == "")
            {
                siteUrl = SPContext.Current.Site.Url;
            }
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        SPList spList = spWeb.Lists.TryGetList(listName);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            string queryStr = GetQueryStr(userList);//
                            if (queryStr == "")//没有请假审批权的用户,隐藏Web部件显示
                            {
                                divData.Visible = false;
                                pagediv.Visible = false;
                                WebPartObj.Visible = false;
                                wbtitle.Visible = false;
                            }
                            else
                            {
                                qry.Query = queryStr;
                                SPListItemCollection listItems = spList.GetItems(qry);

                                if (listItems.Count > 0)
                                {
                                    if (wpFeature == "待办")
                                    {
                                        string approveUrl = WebPartObj.ApproveUrl;
                                        divData.InnerHtml = "您有 <b><a href='" + approveUrl + "'>" + listItems.Count + "</a></b>个请假申请批复!<br/>请及时处理!";
                                        pagediv.Visible = false;
                                    }
                                    else
                                    {
                                        pagediv.Visible = true;
                                        StringBuilder sb = new StringBuilder();
                                        sb.Append("<table  width='100 % ' id='ListArea' border='0' class='t1' cellpadding='0' cellspacing='0'>");
                                        sb.Append("<tr style='font-weight:600;font-size:14px;'><th>标题</th><th>请假人</th><th>申请时间</th></tr>");
                                        foreach (SPListItem item in listItems)
                                        {
                                            string displayUrl = SPContext.Current.Site.RootWeb.Url + "/_layouts/15/CopyUtil.aspx?Use=id&Action=dispform";
                                            //SPSite currentSite = SPContext.Current.Site;
                                            //SPWeb currentWeb = currentSite.OpenWeb();
                                            displayUrl = displayUrl + "&ItemId=" + item.ID + "&ListId=" + spList.ID + "&WebId=" + spWeb.ID + "&SiteId=" + spSite.ID + "&Source=" + UrlEncode(Request.Url.ToString());

                                            sb.Append("<tr>");
                                            sb.Append("<td><a href='" + displayUrl + "'>" + item.Title + "</a></td>");
                                            string temp = item["请假人"].ToString();
                                            temp = temp.Substring(temp.IndexOf("#") + 1);
                                            sb.Append("<td>" + temp + "</td>");
                                            sb.Append("<td>" + string.Format("{0:yyyy/MM/dd}", item["创建时间"]) + "</td>");

                                            sb.Append("</tr>");

                                        }
                                        sb.Append("</table>");
                                        divData.InnerHtml = sb.ToString();
                                    }
                                }
                                else
                                {
                                    divData.InnerHtml = "没有新的请假待审批！<br/>";
                                    pagediv.Visible = false;

                                }
                            }
                        }
                    }
                }
            });
        }

        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }


        #region 打印假条
        /// <summary>
        /// 导出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {

            string id;
            id = Session["ObjChangeID"].ToString();
            try
            {


                if (id != null)
                {

                    string _filePath = PrintDoc(id);
                    if (_filePath != "")
                    {
                        DownloadFile(_filePath, Path.GetFileName(_filePath));//下载文件
                    }
                }
            }
            catch (Exception ex)
            {
                //ZWL.Common.PublicMethod.errorLog("ibtnExport_Click", ex);
            }
        }
        /// <summary>
        /// 打印操作，传入车辆Carcod或者变更ID
        /// </summary>
        /// <param name="id"></param>
        protected string PrintDoc(string id)
        {
            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document doc = new Microsoft.Office.Interop.Word.Document();
            try
            {

                if (id.Length != 36)
                {
                    //objCarInfor.OpenCar(id);
                    string templeteName = "模版.doc", downName = "";///模板文件名称
                    downName =  "请假条.doc";//导出文件名

                    string templeteFile = System.Web.HttpContext.Current.Server.MapPath("~/") + "DocTempalete\\" + templeteName;//模板文件全路径
                    string downFile = System.Web.HttpContext.Current.Server.MapPath("~/") + "ReportFile\\gonghan\\" + downName;///导出文件全路径
                    try
                    {
                        File.Delete(downFile);//删除原有的同名文件
                    }
                    catch
                    {
                    }
                    File.Copy(templeteFile, downFile);//复制模板文件到导出文件对应的文件夹下存档
                    object Obj_FileName = downFile;
                    object Visible = false;
                    object ReadOnly = false;
                    object missing = System.Reflection.Missing.Value;
                    //打开文件
                    doc = app.Documents.Open(ref Obj_FileName, ref missing, ref ReadOnly, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref Visible,
                        ref missing, ref missing, ref missing,
                        ref missing);
                    doc.Activate();
                    #region 给模板填入类容
                    SPListItem item;
                    //光标转到书签
                    object BookMarkName = "Name";
                    object what = Microsoft.Office.Interop.Word.WdGoToItem.wdGoToBookmark;


                    if (app.ActiveDocument.Bookmarks.Exists(BookMarkName.ToString()) == true)
                    {
                        doc.ActiveWindow.Selection.GoTo(ref what, ref missing, ref missing, ref BookMarkName);
                        doc.ActiveWindow.Selection.TypeText(System.DateTime.Now.ToString("yyMMdd"));
                    }
                    BookMarkName = "年";
                    what = Microsoft.Office.Interop.Word.WdGoToItem.wdGoToBookmark;


                    if (app.ActiveDocument.Bookmarks.Exists(BookMarkName.ToString()) == true)
                    {
                        doc.ActiveWindow.Selection.GoTo(ref what, ref missing, ref missing, ref BookMarkName);
                        doc.ActiveWindow.Selection.TypeText(DateTime.Now.ToString("yy"));
                    }
                    BookMarkName = "月";
                    what = Microsoft.Office.Interop.Word.WdGoToItem.wdGoToBookmark;


                    if (app.ActiveDocument.Bookmarks.Exists(BookMarkName.ToString()) == true)
                    {
                        doc.ActiveWindow.Selection.GoTo(ref what, ref missing, ref missing, ref BookMarkName);
                        doc.ActiveWindow.Selection.TypeText(DateTime.Now.ToString("MM"));
                    }
                    BookMarkName = "日";
                    what = Microsoft.Office.Interop.Word.WdGoToItem.wdGoToBookmark;


                    if (app.ActiveDocument.Bookmarks.Exists(BookMarkName.ToString()) == true)
                    {
                        doc.ActiveWindow.Selection.GoTo(ref what, ref missing, ref missing, ref BookMarkName);
                        doc.ActiveWindow.Selection.TypeText(DateTime.Now.ToString("dd"));
                    }
                    ///注意：书签必须不一样才能正确绑定，如果说模板中有需要出现两次的的内容，必须设置成两个标签：
                    ///  如：姓名显示两次，则必须给两个位置都加上标签，以示区别
                    #endregion
                    object IsSave = true;
                    doc.Close(ref IsSave, ref missing, ref missing);///关闭doc文档对象
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);

                    doc = null;

                    object IsSave1 = false;
                    app.Quit(ref IsSave1, ref missing, ref missing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                    app = null;
                    GC.Collect();
                    return downFile;
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                app = null;
                GC.Collect();
                return "";
            }

        }
        ///
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filename">文件名(全路径)</param>
        /// <param name="downname">文件下载名</param>
        protected void DownloadFile(string filename, string downname)
        {
            FileStream f;
            byte[] buffer = new byte[0];
            try
            {
                f = new FileStream(filename, FileMode.Open);
                buffer = new byte[f.Length];
                f.Read(buffer, 0, buffer.Length);
                f.Close();
            }
            catch
            {
                //ZWL.Common.MessageBox.Show(this, "文件不存在！");
                return;
            }

            filename = filename.Replace(@"/", @"\");
            //20121023wangyj

            string saveFileName = "";
            int intStart = filename.LastIndexOf("\\") + 1;
            saveFileName = filename.Substring(intStart, filename.Length - intStart);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";

            string fileType = Path.GetExtension(filename).ToLower();

            switch (fileType)
            {
                case ".asf":
                    System.Web.HttpContext.Current.Response.ContentType = "video/x-ms-asf";
                    break;
                case ".jpg":
                case ".jpeg":
                    System.Web.HttpContext.Current.Response.ContentType = "image/jpeg";
                    break;
                case ".gif":
                    System.Web.HttpContext.Current.Response.ContentType = "image/gif";
                    break;
                case ".pdf":
                    System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                    break;
                case ".avi":
                    System.Web.HttpContext.Current.Response.ContentType = "video/avi";
                    break;
                case ".doc":
                    System.Web.HttpContext.Current.Response.ContentType = "application/msword";
                    break;
                case ".zip":
                    System.Web.HttpContext.Current.Response.ContentType = "application/zip";
                    break;
                case ".rar":
                    System.Web.HttpContext.Current.Response.ContentType = "application/rar";
                    break;
                case ".xls":
                    System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                    break;
                case ".wav":
                    System.Web.HttpContext.Current.Response.ContentType = "audio/wav";
                    break;
                case ".mp3":
                    System.Web.HttpContext.Current.Response.ContentType = "audio/mpeg3";
                    break;
                case ".mpg":
                    System.Web.HttpContext.Current.Response.ContentType = "audio/mpeg";
                    break;
                case ".rtf":
                    System.Web.HttpContext.Current.Response.ContentType = "application/rtf";
                    break;
                case ".htm":
                case ".html":
                    System.Web.HttpContext.Current.Response.ContentType = "text/html";
                    break;
                case ".asp":
                    System.Web.HttpContext.Current.Response.ContentType = "text/asp";
                    break;
                default:
                    System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                    break;
            }

            Response.HeaderEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + Server.UrlEncode(downname));
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");

            //Response.WriteFile(System.Configuration.ConfigurationSettings.AppSettings["TemplatePhysicalPath"].ToString() + pt.Path);
            Response.BinaryWrite(buffer);
            Response.GetHashCode();
            Response.End();
        }
        #endregion




        #region 请假表

        /// <summary>
        /// 根据列表栏的内部名查找显示名
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns></returns>
        public static string getDNameByIName(SPList list, string internalName)
        {
            string dispName = "";
            SPField field = list.Fields.GetFieldByInternalName(internalName);
            dispName = field.Title;
            return dispName;
        }
        protected ArrayList LeaveArray(string lvList,int LeaveId,string apList)
        {
            ArrayList arrayList = new ArrayList();
            Dictionary<int, string> dic = new Dictionary<int, string>();
            string webUrl = WebPartObj.SiteUrl != "" ? WebPartObj.SiteUrl : "";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.Url)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webUrl))
                    {
                        SPList spList = spWeb.Lists.TryGetList(lvList);
                        if (spList != null)
                        {
                            SPListItem item = spList.GetItemById(LeaveId);//查询当前请假信息

                            //DataTable leaveData = createdTable();
                            //创建一行
                            //DataRow row = leaveData.NewRow();
                            //请假人
                            string tempstr =item[getDNameByIName(spList, "CreatedBy")].ToString();                            
                            arrayList.Add(tempstr);
                            //row[0] = tempstr;

                            //性别
                            tempstr = item[getDNameByIName(spList, "Sex")].ToString();
                            arrayList.Add(tempstr);
                            //row[1] = tempstr;

                            //部门
                            tempstr = item[getDNameByIName(spList, "Dept")].ToString();
                            arrayList.Add(tempstr);
                            //row[2] = tempstr;

                            //请假类别
                            tempstr = item[getDNameByIName(spList, "Type")].ToString();
                            arrayList.Add(tempstr);
                            //row[3] = tempstr;

                            //请假事由
                            tempstr = item[getDNameByIName(spList, "Reason")].ToString();
                            arrayList.Add(tempstr);
                            //row[4] = tempstr;

                            SPList appList= spWeb.Lists.TryGetList(apList);//请假审批列表
                            if (appList!=null)
                            {
                                for (int i = 1; i <= 3; i++)//Flag=i*10+1
                                {
                                    string queryStr = @"<Where><And><Eq><FieldRef Name='Flag' /><Value Type='Number'>" + (i * 10 + 1).ToString() + "</Value></Eq><Eq><FieldRef Name='LeaveID' LookupId='True' /><Value Type='Lookup'>" + LeaveId + "</Value></Eq></And></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";
                                    AddListData(arrayList, appList, queryStr);
                                }                                                               
                            }
                            //将此行添加到table中
                            //leaveData.Rows.Add(row);
                        }
                    }
                }
            });
            return arrayList;
        }


        protected void AddListData(ArrayList arrayList, SPList appList,string queryStr)
        {
            string tempstr = "";
            SPQuery qry = new SPQuery();
            qry.Query = queryStr;//查询请假ID为当前请假申请的ID切对应级别的审批记录
            SPListItemCollection listItems = appList.GetItems(qry);
            if (listItems.Count > 0)//具有待审的请假记录
            {
                SPListItem appitem = listItems[0];
                //审批意见
                tempstr = appitem[getDNameByIName(appList, "Option")].ToString();
                arrayList.Add(tempstr);

                //审批人
                tempstr = appitem[getDNameByIName(appList, "CreatedBy")].ToString();
                arrayList.Add(tempstr);

                //审批日期
                DateTime dt = (DateTime)appitem[getDNameByIName(appList, "Created")];

                //审批日期-年
                tempstr = dt.Year.ToString();
                arrayList.Add(tempstr);

                //审批日期-月
                tempstr = dt.Month.ToString();
                arrayList.Add(tempstr);

                //审批日期-日
                tempstr = dt.Day.ToString();
                arrayList.Add(tempstr);
            }
            else//没有该级别的审批记录
            {
                arrayList.Add("");
                arrayList.Add("");
                arrayList.Add("");
                arrayList.Add("");
                arrayList.Add("");
            }
            
        }

        protected DataTable createdTable()
        {
            string[] fields = { "Name", "Sex", "Dept", "Type",  "Year1", "Month1", "Day1", "Year2", "Month2", "Day2", "Reason","LeaveYear", "LeaveMonth", "LeaveDay", "Option1", "Sign1", "SignYear1", "SignMonth1", "SignDay1", "Option2", "Sign2", "SignYear2", "SignMonth2", "SignDay2", "Option3", "Sign3", "SignYear3", "SignMonth3", "SignDay3"};//29项
            DataTable tblleaveData = new DataTable("leaveData");
            for (int i = 0; i < fields.Length; i++)
            {
                tblleaveData.Columns.Add(fields[i], Type.GetType("System.String"));
            }
            return tblleaveData;
         }
        

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string lvList = WebPartObj.ListName;
            int lvId = int.Parse(Request.QueryString["ID"]);
            string apList = WebPartObj.ResultList;
            ArrayList arl = LeaveArray(lvList,lvId,apList);
            StringBuilder sb = new StringBuilder();

            sb.Append("<h2 style='text-align:center'>外国语学院教职工请假审批表</h2>");
            sb.Append("<table class='leavetable' width='600' cellspacing='0' cellpadding='0' border='0' align='center'>");
            sb.Append("<tr><td style='width:100px;'>姓名</td><td>"+arl[0]+ "</td><td>性别</td><td>" + arl[1] + "</td><td>部门</td><td>" + arl[2] + "</td></tr><tr><td>请假类型</td><td colspan='5'><div style='text-align:left'>" + arl[3] + "</div></td></tr>");//姓名、性别、部门、类别共四项，序号：0-3
            sb.Append("<tr><td>请假时间</td><td colspan='5'><div style='text-align:left'>" + arl[4] + " 年 " + arl[5] + " 月 " + arl[6] + " 日 ~ " + arl[7] + " 年 " + arl[8] + " 月 " + arl[9] + " 日</div></td></tr>");//请假时间：开始年月日~结束年月日共六项，序号：4-9
            sb.Append("<tr><td>请假事由</td><td colspan='5'><div style='text-align:left'> " + arl[10] + " </div><br/><div style='text-align:right'>申请人签字：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </div><br><div style='text-align:right'> " + arl[11] + " 年 " + arl[12] + " 月 " + arl[13] + " 日</div></td></tr>");//请假事由：10，请假年月日：11-13
            sb.Append("<tr><td>所在<br/>系（部、办）<br/>意见</td><td colspan='5'><div style='text-align:left'> " + arl[14] + " </div><br/><div style='text-align:right'>负责人签字：" + arl[15] + "  </div><br><div style='text-align:right'> " + arl[16] + " 年 " + arl[17] + " 月 " + arl[18] + " 日</div></td></tr>");//负责人签字5项：14-18
            sb.Append("<tr><td>分管<br/>领导<br/>意见</td><td colspan='5'><div style='text-align:left'> " + arl[19] + " </div><br/><div style='text-align:right'>领导签字： " + arl[20] + " </div><br><div style='text-align:right'> " + arl[21] + " 年 " + arl[22] + " 月 " + arl[23] + " 日</div></td></tr>");//分管领导签字5项：19-23
            sb.Append("<tr><td>人事<br/>领导<br/>意见</td><td colspan='5'><div style='text-align:left'>  " + arl[24] + " </div><br/><div style='text-align:right'>领导签字： " + arl[25] + " </div><br><div style='text-align:right'> " + arl[26] + " 年 " + arl[27] + " 月 " + arl[28] + " 日</div></td></tr>");//人事领导签字5项：24-28
            sb.Append("</table>");

            printDiv.InnerHtml = sb.ToString();
        }
        #endregion
    }
}
