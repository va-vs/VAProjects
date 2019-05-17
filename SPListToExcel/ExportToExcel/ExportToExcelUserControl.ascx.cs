using System;
using System.Web;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text.RegularExpressions;
using System.IO;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;


namespace SPListToExcel.ExportToExcel
{
    public partial class ExportToExcelUserControl : UserControl
    {
        private const string _exportPath = @"/_layouts/15/excel/";
        private const string _imagePath = @"/_layouts/15/images/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SPContext.Current.Web.CurrentUser ==null)
                {
                    lblMsg.Text = "请选登录！";
                    AppAction.Visible = false;
                    return;
                }
                initControl();
            }
            lblMsg.Text = "";
            divSaveAs.InnerHtml = "";
               
            btnExport.Click += btnExport_Click;
            btnOpt.Click += btnOpt_Click;
            btnExportNo.Click += BtnExportNo_Click;
            btnExport.Attributes.Add("onclick", "javascript:document.getElementById('" + divSaveAs.ClientID + "').innerHTML ='正在导出，请稍等……';document.getElementById('" + lblMsg.ClientID + "').innerText='';");
            btnExportNo.Attributes.Add("onclick", "javascript:document.getElementById('" + divSaveAs.ClientID + "').innerHTML ='正在导出，请稍等……';document.getElementById('" + lblMsg.ClientID + "').innerText='';");
        }
       

        #region 事件和方法
        void btnOpt_Click(object sender, EventArgs e)
        {
            if (btnOpt.Text.Replace (" ","") == "全清")
            {
                WriteChkOption(false);
                btnOpt.Text = "全选";

            }
            else
            {
                WriteChkOption(true);
                btnOpt.Text = "全清";
            }
        }
       
        //初始化数据
        private void initControl()
        {
            ddlYear.Items.Clear();
            //年度
            int year = DateTime.Now.Year;
            ddlYear.Items.Add(new ListItem(year.ToString()));
            for (int i = 1; i <= webObj.BeforeYears; i++)
                ddlYear.Items.Add(new ListItem((year - i).ToString()));
            //系部
            FillDepart();
            WriteChkItems();
        }
        private void WriteChkOption(bool chkOpt)
        {

            foreach (ListItem chkItem in chkLists.Items)
            {
                chkItem.Selected = chkOpt;
            }
        }
        //要导出的列表
        private void WriteChkItems()
        {
            string items = webObj.ExportLists.Replace("；", ";").Trim();
            string[] chkItems = Regex.Split(items, ";");
            chkLists.Items.Clear();
            chkLists.RepeatColumns = webObj.RepeatCol;
            foreach (string chkItem in chkItems)
            {
                chkLists.Items.Add(new ListItem(chkItem));
                chkLists.Items[chkLists.Items.Count - 1].Selected = true;
            }
        }
        private void FillDepart()
        {
            ddlDep.Items.Clear();
            ddlDep.Items.Add(new ListItem("全院", "0"));
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb web = spSite.OpenWeb(webObj.SubWebUrl))
                    {
                        SPList list = web.Lists.TryGetList(webObj.DepartList);
                        if (list != null)
                        {
                            string displayName = list.Fields.GetFieldByInternalName("Title").Title;//获取主表的标题显示名称
                            foreach (SPListItem item in list.Items)
                                ddlDep.Items.Add(new ListItem(item[displayName].ToString(), item.ID.ToString()));
                        }
                    }
                }
            });
        }
        //院内教师为空，则按创建者为准
        private void BtnExportNo_Click(object sender, EventArgs e)
        {
            btnExportNo.Enabled = false;
            ExportToExcel(0);
            btnExportNo.Enabled = true;
        }
        override protected void OnPreRender(EventArgs e)
        {
            //doing.Style.Add("visibility", "hidden");
        }
        void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            ExportToExcel(1);
            btnExport.Enabled = true;
        }
        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="opt">1-正常导出；0-录入不正确的</param>
        private void ExportToExcel(int opt = 1)
        {
            string fileName = webObj.ExcelFilename;
            string tempName = webObj.ExcelTempfile;
            DataSet ds = new DataSet();
            DataTable dt = null;
            SPWeb web = SPContext.Current.Web;
            List<int> userIds = new List<int>();
            if (UserID.ResolvedEntities.Count == 0)//用户
                userIds = GetAllUsers(int.Parse(ddlDep.SelectedItem.Value));
            else
            {
                foreach (PickerEntity picker in UserID.ResolvedEntities)
                {
                    SPUser user = web.EnsureUser(picker.Key);
                    userIds.Add(user.ID);
                }

            }
            foreach (ListItem chk in chkLists.Items)
            {
                if (chk.Selected)
                {
                    if (opt == 1)
                        dt = GetYeJiParents(userIds, chk.Text, ddlYear.SelectedItem.Text);
                    else
                        dt = GetYeJiParentsWithNoAuthor(userIds, chk.Text, ddlYear.SelectedItem.Text);

                    if (dt != null)
                        ds.Tables.Add(dt);
                }
            }
            if (ds.Tables.Count > 0)
            {
                lblMsg.Text = "正在导出，请稍等……";
                //btnExport.Enabled = false;
                if (opt == 1)
                    WriteToExcel(tempName, fileName, ds);
                else
                    WriteToExcel(tempName, fileName.Replace(".","不全.") , ds);
                //btnExport.Enabled = true;
                lblMsg.Text = "";
            }
            else
            {
                divSaveAs.InnerHtml = "";
                lblMsg.Text = webObj.NoDataMsg;
            }
        }
        public ExportToExcel webObj { get; set; }
        #endregion
        #region 获取系部教师
        private List<int> GetAllUsers(int deptID)
        {
            string listName = webObj.DepartList;
            string listTeachers = webObj.TeacherList;
            string siteUrl = SPContext.Current.Site.Url;
            SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
            SPListItemCollection retItems = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webObj.SubWebUrl))
                    {
                        SPQuery qry = new SPQuery();
                        SPListItemCollection listItems;

                        #region teachers
                        SPList spTeacherList = spWeb.Lists.TryGetList(listTeachers);
                        if (deptID == 0)
                        {
                            retItems = spTeacherList.Items;
                        }
                        else
                        {
                            qry = new SPQuery();
                            qry.Query = @"<Where><Eq><FieldRef Name='Department' LookupId='True' /><Value Type='Integer'>" + deptID + "</Value></Eq></Where>";
                            listItems = spTeacherList.GetItems(qry);
                            if (listItems.Count > 0)//获取系部下的教师
                            {
                                retItems = listItems;
                            }
                        }
                        #endregion
                    }
                }

            });
            List<int> userIDs = new List<int>();
            if (retItems != null)
            {
                foreach (SPListItem item in retItems)
                {
                    string account = item["工号"].ToString();
                    SPWeb web = SPContext.Current.Web;
                    try
                    {
                        SPUser sUser = web.EnsureUser("ccc\\" + account);
                        userIDs.Add(sUser.ID);
                    }
                    catch { } //用户在AD中并禁用，会出现错误
               }

            }
            return userIDs;
        }
        #endregion
        #region 生成excel相关方法
        private DataTable GetYeJiParents(List<int> userIDs, string listName, string year)
        {
            string siteUrl = SPContext.Current.Site.Url;
            DataTable retDataTable = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(listName);
                        if (spList != null)
                        {
                            SPQuery qry;
                            #region  enumerate user
                            DataRow drNew;
                            bool exportRatio = true;
                            if (listName.Contains("研究生") || listName.Contains("本科") || listName.Contains("毕业论文"))
                                exportRatio = false;
                            foreach (int userID in userIDs)
                            {
                                qry = new SPQuery();
                                qry.Query = @"<Where><And><And><Eq><FieldRef Name='AuthorName' LookupId='True' /><Value Type='Integer'>" + userID + "</Value></Eq><Eq><FieldRef Name='Year' /><Value Type='Text'>" + year + "</Value></Eq></And><Eq><FieldRef Name='Flag'/><Value Type='Number'>1</Value></Eq></And></Where>";
                                SPListItemCollection listItems = spList.GetItems(qry);
                                if (listItems.Count > 0)
                                {
                                    DataTable dt = listItems.GetDataTable();
                                    if (retDataTable == null)
                                    {
                                        retDataTable = dt.Clone();
                                        retDataTable.TableName = listName;
                                        if (exportRatio)
                                            retDataTable.Columns.Add(webObj.RatioCaption, typeof(double));
                                    }
                                    string account;
                                    if (listName == "新开课")//多个人平分课时
                                    {
                                        foreach (DataRow dr in dt.Rows)//
                                        {
                                            SPListItem subITem = listItems.GetItemById((int)dr["ID"]);
                                            SPFieldUserValueCollection users = subITem["AuthorName"] as SPFieldUserValueCollection;

                                            foreach (SPFieldUserValue usr in users)
                                            {
                                                if (usr.LookupId == userID)
                                                {
                                                    drNew = retDataTable.NewRow();
                                                    drNew.ItemArray = dr.ItemArray;
                                                    account = GetAccount(usr.User);//users[0].User错误，只找第一个人
                                                    drNew["AuthorName"] = account;
                                                    drNew["ContentType"] = usr.User.Name;
                                                    drNew["Flag"] = users.Count;
                                                    retDataTable.Rows.Add(drNew);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                       
                                        SPListItemCollection yejiItems = null;
                                        foreach (DataRow dr in dt.Rows)//
                                        {
                                            SPListItem subITem = listItems.GetItemById((int)dr["ID"]);

                                            try
                                            {
                                                SPFieldUserValueCollection users = subITem["AuthorName"] as SPFieldUserValueCollection;

                                                if (users[0].LookupId == userID)//第一作者的是要查的
                                                {
                                                    //2019-4-23业绩项需要导入每个有系数的教师

                                                    if (exportRatio)
                                                    {
                                                        yejiItems = GetYeJiItems(listName, subITem.ID);
                                                        foreach (SPListItem yejiItem in yejiItems)
                                                        {
                                                            drNew = retDataTable.NewRow();
                                                            drNew.ItemArray = dr.ItemArray;
                                                            SPFieldUserValue user = new SPFieldUserValue(spWeb, yejiItem["姓名"].ToString());
                                                            account = GetAccount(user.User);
                                                            drNew["AuthorName"] = account;
                                                            drNew["ContentType"] = user.LookupValue;
                                                            drNew[webObj.RatioCaption] = yejiItem["Ratio"];
                                                            retDataTable.Rows.Add(drNew);
                                                        }
                                                    }
                                                    if (yejiItems == null || yejiItems.Count == 0)
                                                    {
                                                        drNew = retDataTable.NewRow();
                                                        drNew.ItemArray = dr.ItemArray;
                                                        account = GetAccount(users[0].User);
                                                        drNew["AuthorName"] = account;
                                                        drNew["ContentType"] = users[0].User.Name;
                                                        if (exportRatio)
                                                            drNew[webObj.RatioCaption] = 1;
                                                        retDataTable.Rows.Add(drNew);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                //当字段设置中显示名称设置为姓名时，读出来的值为null;只需改动列表设置即可。
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                            if (retDataTable != null)
                            {
                                retDataTable.AcceptChanges();
                                string delFields = "Attachments;MetaInfo;FileLeafRef;Order";//AuthorName";;ContentType去掉相关文档一列
                                #region define column
                                foreach (SPField field in spList.Fields)
                                {
                                    try
                                    {
                                        if (field.ReadOnlyField && !field.InternalName.StartsWith("ClassHour") || delFields.Contains(field.InternalName) || field.Type == SPFieldType.URL)
                                            retDataTable.Columns.Remove(field.InternalName);
                                    }
                                    catch
                                    { }

                                }
                                #endregion

                                //retDataTable.Columns.Remove("ID");//移除Id一列
                                retDataTable.Columns["AuthorName"].Caption = "工号";//移除Id一列
                                retDataTable.Columns["ContentType"].Caption = "姓名";
                                //retDataTable.Columns.Remove("Flag");//移除Id一列
                                retDataTable.Columns["Flag"].Caption = "人数";//新开课使用
                                retDataTable.AcceptChanges();
                            }
                        }
                    }
                }
            });
            return retDataTable;
        }
        //获取满足条件的主表想
        private DataTable GetYeJiParentsWithNoAuthor(List<int> userIDs, string listName, string year)
        {
            string siteUrl = SPContext.Current.Site.Url;
            DataTable retDataTable = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(listName);
                        if (spList != null)
                        {
                            SPQuery qry;
                            #region  enumerate user
                            DataRow drNew;

                            foreach (int userID in userIDs)
                            {
                                qry = new SPQuery();
                                qry.Query = @"<Where><And><And><And><IsNull><FieldRef Name='AuthorName'/></IsNull><Eq><FieldRef Name='Author' LookupId='True'/><Value Type='Integer'>" + userID + "</Value></Eq></And><Eq><FieldRef Name='Year' /><Value Type='Text'>" + year + "</Value></Eq></And><Eq><FieldRef Name='Flag'/><Value Type='Number'>1</Value></Eq></And></Where>";
                                SPListItemCollection listItems = spList.GetItems(qry);
                                if (listItems.Count > 0)
                                {
                                    DataTable dt = listItems.GetDataTable();
                                    if (retDataTable == null)
                                    {
                                        retDataTable = dt.Clone();
                                        retDataTable.TableName = listName;
                                    }
                                    string account;

                                    foreach (DataRow dr in dt.Rows)//
                                    {
                                        SPListItem subITem = listItems.GetItemById((int)dr["ID"]);
                                        //SPFieldUserValueCollection users = subITem["Author"] as SPFieldUserValueCollection;
                                        SPFieldUserValue f = new SPFieldUserValue(spWeb, subITem["Author"].ToString());

                                        drNew = retDataTable.NewRow();
                                        drNew.ItemArray = dr.ItemArray;
                                        account = GetAccount(f.User );
                                        drNew["AuthorName"] = account;
                                        drNew["ContentType"] = f.User.Name;
                                        retDataTable.Rows.Add(drNew);

                                    }

                                }
                            }
                            #endregion
                            if (retDataTable != null)
                            {
                                retDataTable.AcceptChanges();
                                string delFields = "Attachments;MetaInfo;FileLeafRef;Order";//AuthorName";;ContentType去掉相关文档一列
                                #region define column
                                foreach (SPField field in spList.Fields)
                                {
                                    try
                                    {
                                        if (field.ReadOnlyField && !field.InternalName.StartsWith ( "ClassHour" )|| delFields.Contains(field.InternalName) || field.Type == SPFieldType.URL)
                                            retDataTable.Columns.Remove(field.InternalName);
                                    }
                                    catch
                                    { }

                                }
                                #endregion

                                //retDataTable.Columns.Remove("ID");//移除Id一列
                                retDataTable.Columns["AuthorName"].Caption = "工号";//移除Id一列
                                retDataTable.Columns["ContentType"].Caption = "姓名";
                                //retDataTable.Columns.Remove("Flag");//移除Id一列
                                retDataTable.Columns["Flag"].Caption = "人数";//新开课使用
                                retDataTable.AcceptChanges();
                            }
                        }
                    }
                }
            });
            return retDataTable;
        }
        //获取登陆用户的账号
        public static string GetAccount(SPUser currentUser)
        {
            string loginName = currentUser.LoginName;
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        private string GetParentTitle(string listName)
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(listName);
            if (list != null)
            {
                string displayName = list.Fields.GetFieldByInternalName("Title").Title;//获取主表的标题显示名称
                return displayName;
            }
            else
                return "";
            //HSSFWorkbook hssfworkbook;
            //using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            //{
            //    hssfworkbook = new HSSFWorkbook(file);
            //}
            //IWorkbook wk = WorkbookFactory.Create(fs);   //使用接口，自动识别excel2003/2007格式
            //强制Excel重新计算表中所有的公式
            //sheet1.ForceFormulaRecalculation = true;
        }
        //NPOI.OOXML.DLL　　 NPOI.XSSF　　 Excel 2007(xlsx)格式读写库　
　　      //基于现在的模板文件生成Excel
        private void WriteToExcel(string tempName, string fileName, DataSet ds)
        {
            IWorkbook book = null;
            string _filePath = Server.MapPath(_exportPath) + tempName;
            using (FileStream fs = File.Open(_filePath, FileMode.Open, FileAccess.ReadWrite)) 
            {
                book = WorkbookFactory.Create(fs);
                ISheet wSheet;//define worksheet

                ICellStyle cStyle = book.CreateCellStyle();//define style
                cStyle.DataFormat = 194;//HSSFDataFormat.GetBuiltinFormat("###0.00");//(short)CellType.NUMERIC;

                foreach (DataTable dt in ds.Tables)
                {
                    wSheet = book.GetSheet(dt.TableName);//打开指定的工作簿

                    #region f填充数据
                    NPOI.SS.UserModel.IRow rowTitle = wSheet.GetRow(0);
                    NPOI.SS.UserModel.IRow rowtemp;
                    DataRow dr;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        rowtemp = wSheet.GetRow(i + 1);//先获取现有的行，如果为空，则创建
                        if (rowtemp == null)
                            rowtemp = wSheet.CreateRow(i + 1);
                        ICell rowCell;
                        string headTitle;
                        for (int j = 0; j < rowTitle.LastCellNum; j++)
                        {
                            if (rowTitle.GetCell(j) == null) break;
                            headTitle = rowTitle.GetCell(j).ToString();
                            foreach (DataColumn dc in dt.Columns)//找到指定的列
                            {
                                if (dc.Caption == headTitle)
                                {
                                    rowCell = rowtemp.GetCell(j);
                                    if (rowCell == null)
                                        rowCell = rowtemp.CreateCell(j);
                                    dr = dt.Rows[i];
                                    if (!dr.IsNull(dc.ColumnName))
                                    {
                                        if (dc.DataType.Name == "Double" )
                                        {
                                            rowCell.SetCellValue(double.Parse(dr[dc.ColumnName].ToString()));
                                            rowCell.CellStyle = cStyle;
                                        }
                                        
                                        else
                                            rowCell.SetCellValue(dr[dc.ColumnName].ToString());
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //调整列宽
                    for (int j = 0; j < rowTitle.LastCellNum; j++)
                    {
                        wSheet.AutoSizeColumn(j);
                    }
                    #endregion
                }
            }
            //保存到本地文件
            _filePath = Server.MapPath(_exportPath) + fileName;
            using (FileStream fs1 = File.Open(_filePath, FileMode.Create, FileAccess.ReadWrite))  
            {
                book.Write(fs1);
            }
            book = null;
            divSaveAs.InnerHtml = "<font color='black'>Excel文件已生成，请</font> <a href='" + _exportPath + fileName + "'>单击下载</a> ";
        }
        //基于空白的Excel文件写入，没有模板
        private void WriteToExcel(  string fileName, DataSet ds)
        {
            loading.Visible = true;
            IWorkbook book = null;

            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                book = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                book = new HSSFWorkbook();

            ISheet wSheet;//define worksheet
            foreach (DataTable dt in ds.Tables)
            {
                wSheet = book.CreateSheet(dt.TableName);//

                #region 列标题及样式
                ICellStyle headStyle = book.CreateCellStyle();
                headStyle.Alignment = HorizontalAlignment.Center;
                IFont font = book.CreateFont();
                font.FontHeightInPoints = webObj.FontSize;
                font.FontName = "宋体";
                font.Boldweight = 700;
                headStyle.SetFont(font);

                NPOI.SS.UserModel.IRow row1 = wSheet.CreateRow(0);
                NPOI.SS.UserModel.IRow rowtemp;


                #endregion
                #region 正文样式
                ICellStyle itemStyle = book.CreateCellStyle();
                itemStyle.Alignment = HorizontalAlignment.Left;
                font = book.CreateFont();
                font.FontHeightInPoints = webObj.FontSize;
                font.FontName = "宋体";
                itemStyle.SetFont(font);
                #endregion
                #region f填充数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    rowtemp = wSheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        rowtemp.CreateCell(j).SetCellValue(dt.Rows[i][dt.Columns[j].ColumnName].ToString());
                        rowtemp.GetCell(j).CellStyle = itemStyle;
                    }
                }
                #endregion
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row1.CreateCell(i).SetCellValue(dt.Columns[i].Caption);
                    row1.GetCell(i).CellStyle = headStyle;
                    //设置列宽
                    wSheet.AutoSizeColumn(i);
                }
            }
            //保存到本地文件
            string _filePath = Server.MapPath(_exportPath) + fileName;
            using (FileStream fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            {
                book.Write(fs);
            }
            book = null;
            loading.Visible = false;
            divSaveAs.InnerHtml = "Excel文件已生成，请<a href='" + _exportPath + fileName + "'>单击下载</a>";
        }
        #endregion
        #region  nouserd
        private void SaveAsExcel(string fileName)
        {
            //之前的方法

            string _filePath = Server.MapPath(_exportPath) + fileName;
            using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                long filesize = fs.Length;
                int i = Convert.ToInt32(filesize);
                byte[] fileContent = new byte[i];
                fs.Read(fileContent, 0, i);
                fs.Close();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", Server.UrlEncode(fileName) + ".xlsx"));

                //Response.ContentType = "application/vnd.ms-exce";
                //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", Server.UrlEncode(fileName) + ".xls"));
                Response.BinaryWrite(fileContent);
                Response.End();
            }
        }
        private DataTable GetYeJiParents(int userID, string listName, string year)
        {
            string siteUrl = SPContext.Current.Site.Url;
            DataTable retDataTable = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(listName);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            qry.Query = @"<Where><And><And><Eq><FieldRef Name='AuthorName' LookupId='True' /><Value Type='Integer'>" + userID + "</Value></Eq><Eq><FieldRef Name='Year' /><Value Type='Text'>" + year + "</Value></Eq></And><Eq><FieldRef Name='Flag'/><Value Type='Number'>1</Value></Eq></And></Where>";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)// 获取主表满足条件的数据
                            {
                                DataTable dt = listItems.GetDataTable();
                                string delFields = "Attachments;MetaInfo;FileLeafRef;Order;ContentType";//AuthorName";
                                #region define column
                                foreach (SPField field in spList.Fields)
                                {
                                    try
                                    {
                                        if (field.ReadOnlyField && field.InternalName != "ID" || delFields.Contains(field.InternalName))
                                            dt.Columns.Remove(field.InternalName);
                                    }
                                    catch
                                    { }
                                }
                                #endregion
                                #region enumerate
                                DataRow drNew;
                                retDataTable = dt.Clone();
                                retDataTable.TableName = listName;

                                foreach (DataRow dr in dt.Rows)//遍历主表,是否已经审核
                                {
                                    SPListItem subITem = listItems.GetItemById((int)dr["ID"]);

                                    SPFieldUserValueCollection users = subITem["AuthorName"] as SPFieldUserValueCollection;
                                    if (users[0].LookupId == userID)//第一作者的是要查的
                                    {
                                        drNew = retDataTable.NewRow();
                                        drNew.ItemArray = dr.ItemArray;
                                        retDataTable.Rows.Add(drNew);
                                    }
                                    
                               }
                                retDataTable.AcceptChanges();
                                #endregion
                                retDataTable.Columns.Remove("ID");//移除Id一列
                                retDataTable.Columns.Remove("AuthorName");//移除Id一列
                                retDataTable.AcceptChanges();

                            }
                        }
                    }
                }
            });
            return retDataTable;
        }
        private bool CheckAudit(string listName, int parentID)
        {
            string siteUrl = SPContext.Current.Site.Url;
            bool result = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(listName + "业绩");
                        string lookupInternalName = spList.Fields.GetField(GetParentTitle(listName)).InternalName;
                        string strAction;
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            strAction = "审核";
                            qry.Query = @"<Where><And><Eq><FieldRef Name='Action' /><Value Type='Choice'>" + strAction + "</Value></Eq><Eq><FieldRef Name='" + lookupInternalName + "' LookupId='True' /><Value Type='Lookup'>" + parentID + "</Value></Eq></And></Where>";

                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)//已经审核，获取指定用户的业绩
                                result = true;
                            else
                                result = false;
                        }
                    }
                }
            });
            return result;
        }
        /// <summary>
        /// 获取已经录入的业绩点数，1、是否审核，2、返回业绩
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="state"></param>
        /// <param name="authorID">查询业绩信息</param>
        private SPListItemCollection GetYeJiItems(string listName, int parentID)
        {
            string siteUrl = SPContext.Current.Site.Url;
            //SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
            SPListItemCollection retItems = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(listName + "业绩");
                        if (spList.Fields.ContainsFieldWithStaticName("Ratio"))
                        {
                            string lookupInternalName = spList.Fields.GetField(GetParentTitle(listName)).InternalName;
                            string strAction;
                            if (spList != null)
                            {
                                SPQuery qry = new SPQuery();
                                strAction = "录入";
                                qry.Query = @"<Where><And><And><Gt>
         <FieldRef Name='Ratio' />
         <Value Type='Number'>0</Value>
      </Gt><Eq><FieldRef Name='Action' /><Value Type='Choice'>" + strAction + "</Value></Eq></And><Eq><FieldRef Name='" + lookupInternalName + "' LookupId='True' /><Value Type='Lookup'>" + parentID + "</Value></Eq></And></Where>";
                                SPListItemCollection listItems = spList.GetItems(qry);

                                retItems = listItems;
                            }
                        }
                    }
                }
            });
            return retItems;
        }
        #endregion
    }
}
