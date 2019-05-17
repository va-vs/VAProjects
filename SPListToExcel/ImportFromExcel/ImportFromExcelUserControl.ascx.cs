using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SPListToExcel.ImportFromExcel
{
    public partial class ImportFromExcelUserControl : UserControl
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                initControl();
            btnImport.Click += BtnImport_Click;
            btnImport.Attributes.Add("onclick", "javascript:document.getElementById('" + lblMsg.ClientID + "').innerText='正在导入，请稍等……';");
        }
        //从excel导数据到列表
        private void BtnImport_Click(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            List<string> userIds = new List<string>();
            if (UserID.ResolvedEntities.Count == 0)//用户
                userIds = GetAllUsersIdCard(int.Parse(ddlDep.SelectedItem.Value));
            else
            {
                foreach (PickerEntity picker in UserID.ResolvedEntities)
                {
                    SPUser user = web.EnsureUser(picker.Key);
                    userIds.Add(GetAccount(user));
                }

            }
            DataTable dt = ReadExcel();
            if (dt!=null)
            {
                int total= WriteDataToList(dt, userIds);
                lblMsg.Text = "本次导入 " + total +" 条数据！";
            }

        }
        #endregion
        #region 方法
        public static string GetAccount(SPUser currentUser)
        {
            string loginName = currentUser.LoginName;
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        public  ImportFromExcel webObj {get;set;}
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
        }
        private void FillDepart()
        {
            ddlDep.Items.Clear();
            ddlDep.Items.Add(new ListItem("全院", "0"));
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb web = spSite.OpenWeb(webObj.SubWebUrl))
                    {
                        if (!web.Exists)
                        {
                            lblMsg.Text =webObj.SubWebUrl + "   不存在！";
                            return;
                        }
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
        #endregion
        #region
        #region 获取系部教师
        private List<string> GetAllUsersIdCard(int deptID)
        {
            string listName = webObj.DepartList;
            string listTeachers = webObj.TeacherList;
            string siteUrl = SPContext.Current.Site.Url;
            SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
            SPListItemCollection retItems = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
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
            List<string> userIDs = new List<string>();
            if (retItems != null)
            {
                foreach (SPListItem item in retItems)
                {
                    string account = item["工号"].ToString();
                    //SPWeb web = SPContext.Current.Web;
                    //SPUser sUser = web.EnsureUser("ccc\\" + account);
                    userIDs.Add(account);
                }

            }
            return userIDs;
        }
        #endregion

        private DataTable ReadExcel()
        {
            if (FileUpload1.PostedFile == null)
            {
                lblMsg.Text = "先选择导入的业绩汇总文档！";
                return null;
            }
            else if (!FileUpload1.FileName.EndsWith("xlsm"))
            {
                lblMsg.Text = "先选择带有（.xlsm）后缀的业绩汇总文档！";
                return null;
            }
            Stream fs = FileUpload1.PostedFile.InputStream;
            IWorkbook book = WorkbookFactory.Create(fs);

            ISheet wSheet;//define worksheet
            wSheet = book.GetSheet("计算总表");//打开指定的工作表
            if (wSheet == null)
            {
                lblMsg.Text = "业绩汇总文档有误，请重新选择！";
                return null;
            }
            DataTable dt = new DataTable();

            DataRow dr;
            int lStart = 3;//导入的开始行
            int lEnd = wSheet.LastRowNum;//导入的结束行
            #region 判断导入的条件

            #endregion
            #region 读取标题以外的数据
            string headTitle;
            IRow rowTitle = wSheet.GetRow(0);//标题行
            IRow rowFirst = wSheet.GetRow(1);
             System.Collections.IEnumerator rows = wSheet.GetRowEnumerator();
            string preFix = rowFirst.GetCell(1).ToString();
            for (int j = 1; j < rowTitle.LastCellNum; j++)
            {
                if (rowFirst.GetCell(j).ToString() != "")
                {
                    headTitle = rowFirst.GetCell(j).ToString();//存在多列合并的情况
                    if (rowTitle.GetCell(j).ToString() != "")
                        preFix = rowTitle.GetCell(j).ToString();
                }
                else
                    headTitle = rowTitle.GetCell(j).ToString();

                headTitle = headTitle.Replace("\n","");
                preFix = preFix.Replace("\n","");

                if (preFix.StartsWith("本科课时"))
                    dt.Columns.Add("本科" + headTitle);
                else if (preFix.StartsWith("研究生课时") && !headTitle.Contains("研究生") )
                    dt.Columns.Add("研究生" + headTitle);
                else if (preFix.StartsWith("结余"))
                    dt.Columns.Add(preFix);
                else
                    dt.Columns.Add(headTitle);


            }
            IRow rowtemp;
            for (int i = lStart - 1; i < lEnd; i++)
            {
                rowtemp = wSheet.GetRow(i);//先获取现有的行，如果为空，则创建
                if (rowtemp == null||rowtemp.GetCell(0).ToString ().StartsWith("合计"))
                    break;
                

                dr = dt.NewRow();
                ICell item;
                string cellValue="";
                for (int j = 1; j < rowTitle.LastCellNum; j++)
                {
                    headTitle = dt.Columns[j - 1].ColumnName;// rowTitle.GetCell(j).ToString();
                    item = rowtemp.GetCell(j);
                    if (item.CellType == CellType.Formula)
                    {
                        switch (item.CachedFormulaResultType)
                        {
                            case CellType.Boolean:
                                dr[headTitle] = item.BooleanCellValue;
                                break;
                            case CellType.Error:
                                //dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                break;
                            case CellType.Numeric:
                                if (DateUtil.IsCellDateFormatted(item))
                                {
                                    dr[headTitle] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                }
                                else
                                {
                                    dr[headTitle] = item.NumericCellValue;
                                }
                                break;
                            case CellType.String:
                                string str = item.StringCellValue;
                                if (!string.IsNullOrEmpty(str))
                                {
                                    dr[headTitle] = str.ToString();
                                }
                                else
                                {
                                    dr[headTitle] = null;
                                }
                                break;
                            case CellType.Unknown:
                            case CellType.Blank:
                            default:
                                dr[headTitle] = string.Empty;
                                break;
                        }
                    }
                    else
                    {
                        cellValue = item == null ? "" : item.ToString();
                        dr[headTitle] = cellValue;
                    }

                }
                dt.Rows.Add(dr);
            }
            #endregion
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name=""></param>
        private int WriteDataToList(DataTable dTable, List<string> userIDs)
        {
            int i=0;
            SPSecurity.RunWithElevatedPrivileges(delegate()
          {
              try
              {
                  using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                  {
                      using (SPWeb sWeb = spSite.AllWebs[SPContext.Current.Web.ID])
                      {
                          sWeb.AllowUnsafeUpdates = true;
                          SPList sList = sWeb.Lists[webObj.ListName];
                          SPListItem item = null;
                          int itemID;
                          string year;
                          DataRow dr;
                          DataRow[] drs;
                          foreach (string  idCard in userIDs )
                          {
                              drs = dTable.Select("工号='"+idCard +"'");
                              if (drs.Length == 0)
                                  continue;
                              dr = drs[0];
                              if (!userIDs.Contains(idCard))
                                  continue;
                              year = ddlYear.SelectedItem.Text ;
                              itemID = GetItemByIDCardAndYear(idCard, year);
                              if (itemID == 0)
                              {
                                  item = sList.AddItem();
                                  item["年度"] = year ;
                              }
                              else
                              {
                                  item = sList.GetItemById(itemID);
                              }
                              foreach (SPField  dc in sList.Fields)
                              {
                                  if (dTable.Columns.Contains (dc.Title ))//列表按列遍历
                                      item[dc.Title ] = dr[dc.Title ];
                              }
                              item.Update();
                              i = i + 1;
                          }
                          sWeb.AllowUnsafeUpdates = false;
                      }
                  }
                 
              }
              catch (Exception ex)
              {
                  lblMsg.Text = ex.ToString();
              }
          });
            return i;
        }
        /// <summary>
        /// 是否存在指定工号和学度的数据
        /// </summary>
        /// <returns></returns>
        private int GetItemByIDCardAndYear(string IDCard, string Year)
        {
            int itemID = 0;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID ))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.ListName);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            qry.Query = @"<Where><And><Eq><FieldRef Name='IDCard' /><Value Type='Text'>" + IDCard  + "</Value></Eq><Eq><FieldRef Name='Year' /><Value Type='Text'>" + Year + "</Value></Eq></And></Where>";

                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                itemID = listItems[0].ID ;
                        }
                    }
                }
            });
            return itemID ;
        }
        /// <summary>
        /// 创建者信息，通过excel工作表的名称
        /// </summary>
        /// <param name="wSheet"></param>
        /// <param name="userID"></param>
        private void WriteToList(ISheet wSheet, int userID)
        {

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                {
                    using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                    {
                        using (SPWeb sWeb = spSite.AllWebs[SPContext.Current.Web.ID ])
                        {
                            sWeb.AllowUnsafeUpdates = true;
                            SPList sList = sWeb.Lists[webObj.ListName];
                            SPListItem item = null;
                            #region 读取标题以外的数据
                            IRow rowTitle = wSheet.GetRow(0);
                            IRow rowtemp;
                            for (int i = 0; i < wSheet.LastRowNum; i++)
                            {
                                rowtemp = wSheet.GetRow(i + 1);//先获取现有的行，如果为空，则创建
                                if (rowtemp == null)
                                    return;
                                item = sList.AddItem();
                                string headTitle;
                                string cellValue;
                                for (int j = 1; j < rowTitle.LastCellNum; j++)
                                {
                                    headTitle = rowTitle.GetCell(j).ToString();
                                    cellValue = rowtemp.GetCell(j) == null ? "" : rowtemp.GetCell(j).ToString();
                                    switch (headTitle)
                                    {
                                        case "计划开始":
                                        case "实际开始":
                                            {
                                                if (cellValue != "")
                                                {
                                                    try
                                                    {
                                                        cellValue = rowtemp.GetCell(j).DateCellValue.ToShortTimeString();
                                                        string date = rowtemp.GetCell(0).DateCellValue.ToShortDateString();
                                                        DateTime dt = DateTime.Parse(date + " " + cellValue);
                                                        item[headTitle] = dt;
                                                    }
                                                    catch
                                                    { }
                                                }
                                                break;
                                            }
                                        case "计划时长":
                                        case "实际时长":
                                            {
                                                try
                                                {
                                                    item[headTitle] = cellValue == "" ? 0 : int.Parse(cellValue);
                                                }
                                                catch
                                                {
                                                }
                                                break;
                                            }
                                        case "结果":
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                item[headTitle] = cellValue;
                                                break;
                                            }
                                    }
                                }
                                item["Author"] = userID;
                                item["Editor"] = userID;
                                item.Update();
                            }
                            #endregion
                            sWeb.AllowUnsafeUpdates = false;
                        }
                    }
                    lblMsg.Text = "导入完成！";
                }
                catch (Exception ex)
                {
                    lblMsg.Text = ex.ToString();
                }
            });
        }
        #endregion
    }
}
