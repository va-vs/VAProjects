using System;
using System.IO;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using NPOI.SS.UserModel;

namespace VATaskWebPart.ImportFromExcel
{
    public partial class ImportFromExcelUserControl : UserControl
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                dtStart.SelectedDate = DateTime.Today;
                dtEnd.SelectedDate = DateTime.Today;
            }
            if (SPContext.Current.Web.CurrentUser == null)
            {
                lblMsg.Text = "请选登录";
                divUpload.Visible = false;
                return;
            }
            bool isExits = ListExits();
            if (!isExits)
            {
                lblMsg.Text = webObj.ListName + "  列表不存在！";
                divUpload.Visible = false;
                return;
            }
            lblMsg.Text = "";
            divUpload.Visible = true;

            rbOpt.SelectedIndexChanged += rbOpt_SelectedIndexChanged;
            btnImport.Click += btnImport_Click;
        }

        void rbOpt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbOpt.SelectedItem.Value =="1")//datetime
            {
                spanDate.Visible = true;
                spanLine.Visible = false;
             }
            else if (rbOpt.SelectedItem.Value =="2")//line
            {
                spanDate.Visible = false;
                spanLine.Visible = true;
             }
            else//all
            {
                spanDate.Visible = false;
                spanLine.Visible = false ;
            }
        }

        void btnImport_Click(object sender, EventArgs e)
        {
            if (FileUpload1.FileName != "")
            {
                ISheet wSheet = ReadExcel();
                string account = wSheet.SheetName;
                int userID = GetAuthorID(account);
                if (userID == 0)
                    lblMsg.Text = "工作表名称有误，请重新改正！";
                else
                {
                    //WriteToList(wSheet, userID);
                    DataTable dt = ReadToTabel(wSheet, userID);
                    WriteDataToList(dt, userID.ToString ());

                }
            }
            else
                lblMsg.Text = "先选择要导入到列表的Excel文件";
        }
        #endregion
        #region 属性
        public ImportFromExcel webObj { get; set; }

        #endregion
        #region 方法
        //获取导入数据的创建者信息，
        private int GetAuthorID(string account)
        {
            SPWeb myWeb = SPContext.Current.Site.OpenWeb(webObj.WebUrl);
            int id = 0;
            try
            {
                SPUser s = myWeb.EnsureUser(account);
                id = s.ID;
            }
            catch
            {

            }
            return id;
        }
        private ISheet ReadExcel()
        {
            Stream fs = FileUpload1.PostedFile.InputStream;
            IWorkbook book = WorkbookFactory.Create(fs);

            ISheet wSheet;//define worksheet
            wSheet = book.GetSheetAt(0);//打开指定的工作表
            return wSheet;
        }
        private DataTable ReadToTabel(ISheet wSheet, int userID)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("计划开始", typeof(DateTime));
            dt.Columns.Add("计划时长", typeof(int)); 
            dt.Columns.Add("实际开始", typeof(DateTime)); 
            dt.Columns.Add("实际时长", typeof(int));
            dt.Columns.Add("操作");
            dt.Columns.Add("对象");
            dt.Columns.Add("活动类型");
            dt.Columns.Add("备注");
            dt.Columns.Add("创建者");
            dt.Columns.Add("修改者");
            DataRow dr;
            int lStart =2;//导入的开始行
            int lEnd = wSheet.LastRowNum;//导入的结束行
            #region 判断导入的条件
            if (rbOpt.SelectedItem.Value  =="2")//按行号
            {
                lStart = int.Parse (txtStart.Text);
                lEnd = int.Parse(txtEnd.Text ) ;
                if (lEnd > wSheet.LastRowNum)
                    lEnd = wSheet.LastRowNum;
            }
            #endregion
            #region 读取标题以外的数据
            IRow rowTitle = wSheet.GetRow(0);
            IRow rowtemp;
            for (int i = lStart-1 ; i < lEnd ; i++)
            {
                rowtemp = wSheet.GetRow(i);//先获取现有的行，如果为空，则创建
                if (rowtemp == null)
                    break;
                dr = dt.NewRow();
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
                                        DateTime dtValue = DateTime.Parse(date + " " + cellValue);
                                        dr[headTitle] = dtValue;
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
                                    dr[headTitle] = cellValue == "" ? 0 : int.Parse(cellValue);
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
                                dr[headTitle] = cellValue;
                                break;
                            }
                    }
                }
                dr["创建者"] = userID;
                dr["修改者"] = userID;
                dt.Rows.Add(dr);
            }
            #endregion
            if (rbOpt.SelectedItem.Value =="1")//按日期
            {
                DataTable dtAll = dt.Copy();
                DataSet ds = new DataSet();
                DataRow[] drs = dtAll.Select("计划开始>='" + dtStart.SelectedDate.ToShortDateString() + "' and 计划开始<'" + dtEnd.SelectedDate.AddDays (1).ToShortDateString() + "'");
                ds.Merge(drs);
                dt = ds.Tables[0].Copy(); 
            }
            return dt;
        }
        private void WriteDataToList(DataTable dTable,string userID)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
          {
              try
              {
                  using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                  {
                      using (SPWeb sWeb = spSite.AllWebs[webObj.WebUrl])
                      {
                          sWeb.AllowUnsafeUpdates = true;
                          SPList sList = sWeb.Lists[webObj.ListName];
                          SPListItem item = null;
                          string planDate;
                          string planDuring;
                          foreach (DataRow dr in dTable.Rows )
                          {
                              planDate = dr["计划开始"].ToString ();
                              planDuring = dr["计划时长"].ToString();
                              item = GetItemByPlanDate(planDate, planDuring, userID);
                              if (item ==null)
                              {
                                  item = sList.AddItem();
                                  foreach (DataColumn dc in dTable.Columns)
                                  {
                                      if (!dr.IsNull(dc.Caption))
                                          item[dc.Caption] = dr[dc.Caption];
                                  }
                                  item.Update();
                              }

                          }
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
                        using (SPWeb sWeb = spSite.AllWebs[webObj.WebUrl])
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
        /// <summary>
        /// 通过计划开始和计划时长判断是否有重复数据,PlanDate,PlanDuring
        /// </summary>
        /// <returns></returns>
        private SPListItem GetItemByPlanDate(string planDate,string during,string userID)
        {
            SPListItem item = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webObj.WebUrl))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.ListName);
                        if (spList != null)
                        {
                            planDate= SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(planDate));
                            SPQuery qry = new SPQuery();
                            qry.Query = @"<Where><And> <And><Eq>
               <FieldRef Name='PlanDate' />
               <Value Type='DateTime' IncludeTimeValue='TRUE'>" + planDate + "</Value></Eq> <Eq> <FieldRef Name='PlanDuring' /><Value Type='Number'>" + during + "</Value></Eq></And><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Lookup'>" + userID + "</Value></Eq></And></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";

                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                item = listItems[0];
                        }
                    }
                }
            });
            return item;
        }
        private bool ListExits()
        {
            bool isExits = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webObj.WebUrl))
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
        #endregion
    }
}
