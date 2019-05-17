using System;
using System.IO;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace VATaskWebPart.ImportFromSchedule
{
    public partial class ImportFromScheduleUserControl : UserControl
    {
        //实现日程和个人学习助手 数据的同步，将日程中的数据同步到个人学习助手中
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
            if (rbOpt.SelectedItem.Value == "1")//datetime
            {
                spanDate.Visible = true;
            }
            else//all
            {
                spanDate.Visible = false;
            }
        }

        void btnImport_Click(object sender, EventArgs e)
        {
            int userID = GetAuthorID ("");
            if (userID == 0)
                lblMsg.Text = "用户不存在！";
            else
            {
                WriteDataToList( userID.ToString());

            }
        }
        #endregion
        #region 属性
        public ImportFromSchedule  webObj { get; set; }

        #endregion
        #region 方法
        //获取导入数据的创建者信息，当前登录用户，自己导入自己的数据
        private int GetAuthorID(string account)
        {
            SPWeb myWeb = SPContext.Current.Web;
            int id = 0;
            if (account == "")
            {
                id = myWeb.CurrentUser.ID;
                return id;
            }
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
        //按条件查找某个用户的日程数据,按分配对象进行查找
        private SPListItemCollection GetScheduleItems(string  userID)
        {
            SPListItemCollection items = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.SchedulelList );
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            if (rbOpt.SelectedItem.Value == "1")//按日期
                            {
                                qry.Query = @"<Where><And><And><Geq><FieldRef Name='PlanStart' /><Value Type='DateTime'>" + SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse ( dtStart.SelectedDate.ToShortDateString())) + "</Value></Geq><Lt><FieldRef Name = 'PlanStart' /><Value Type = 'DateTime' > " + SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse ( dtEnd.SelectedDate.AddDays(1).ToShortDateString())) + " </Value></Lt></And><Eq><FieldRef Name='AssignedTo' LookupId='True' /><Value Type='Lookup'>" + userID + "</Value></Eq></And></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";
                            }
                            else
                            {
                                qry.Query = @"<Where><Eq><FieldRef Name='AssignedTo' LookupId='True' /><Value Type='Lookup'>" + userID + "</Value></Eq></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";

                            }
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                items = listItems;
                        }
                    }
                }
            });
            return items;
        }

        /// <summary>
        /// 指定条件的日程数据，如果在个人学习助手中不存在，则写入，同时返回ID并写入日程对应的数据，如果存在，则更新
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private void WriteDataToList(string userID)
        {
            SPListItemCollection scheduleItems = GetScheduleItems(userID);
            if (scheduleItems == null)
            {
                lblMsg.Text = "没有符合条件的导入数据！";
                return;
            }

            SPSecurity.RunWithElevatedPrivileges(delegate ()
        {
            try
            {

                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb sWeb = spSite.AllWebs[SPContext.Current.Web.ID])
                    {
                        sWeb.AllowUnsafeUpdates = true;
                        SPList sList = sWeb.Lists[webObj.ListName];
                        SPList sScheduleList = sWeb.Lists[webObj.SchedulelList];
                        SPListItem item = null;
                        string planDate;
                        string planDuring;
                        SPListItem taskItem;
                        int planID;
                        foreach (SPListItem scheItem in scheduleItems)
                        {
                            planDate = scheItem["计划开始"].ToString();
                            planDuring = scheItem["计划时长"].ToString();
                            planID = scheItem["PlanID"] != null ? int.Parse(scheItem["PlanID"].ToString().Substring(0, scheItem["PlanID"].ToString().IndexOf(";#"))) : 0;
                            item = GetItemByPlanDate(planDate, planDuring, userID, planID);
                            if (item == null)//不存在，添加，否则编辑
                                item = sList.AddItem();
                            else
                                item = sList.GetItemById((int)item["ID"]);
                            item["计划开始"] = scheItem["计划开始"];
                            item["计划时长"] = scheItem["计划时长"];
                            int taskID = int.Parse(scheItem["Task"].ToString().Substring(0, scheItem["Task"].ToString().IndexOf(";#")));
                            taskItem = GetTaskByID(taskID ,"Task",webObj.SchedulelList );
                            if (taskItem != null)
                            {
                                item["操作"] = taskItem["操作"];
                                item["对象"] = taskItem["对象"];
                            }

                            item["实际开始"] = scheItem["实际开始"];
                            item["实际时长"] = scheItem["实际时长"];
                            item["Author"] = userID;
                            item["Editor"] = userID;
                            item.Update();
                            planID = item.ID;//更新关系ID
                            SPListItem upItem = sScheduleList.GetItemById((int)scheItem["ID"]);
                            upItem["PlanID"] = planID;
                            upItem.Update();

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
        /// 获取日程所对应的任务列表项，为了获取操作和对象 
        /// </summary>
        /// <param name="taskID">通过查阅项解析出的ID值</param>
        /// <param name="spFieldName">查询项的字段内部名称</param>
        /// <param name="listName">包含查阅项的列表名称</param>
        /// <returns></returns>
        private SPListItem GetTaskByID(int taskID,string spFieldName,string listName)
        {
            SPListItem item = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList1 = spWeb.Lists.TryGetList(listName );//有查阅项的列表
                        SPFieldLookup task = spList1.Fields.GetFieldByInternalName(spFieldName) as SPFieldLookup ;
                        SPList spList = spWeb.Lists[new Guid(task.LookupList)];
                        if (spList != null)
                        {
                            SPListItem  taskItem = spList.GetItemById(taskID);
                            if (taskItem != null)
                                item = taskItem;
                        }
                    }
                }
            });
            return item;
        }
        /// <summary>
        /// 通过计划开始和计划时长判断当前用户是否有重复数据,PlanDate,PlanDuring
        /// 如果已经添加，则通过ID获取
        /// </summary>
        /// <returns></returns>
        private SPListItem GetItemByPlanDate(string planDate, string during, string userID, int planID)
        {
            SPListItem item = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.ListName);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();

                            planDate = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(planDate));
                            qry.Query = @"<Where><Or><Eq><FieldRef Name='ID' /><Value Type='Counter'>" 
+ planID.ToString() + "</Value></Eq><And><And><Eq><FieldRef Name='PlanDate' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + planDate + "</Value></Eq><Eq><FieldRef Name='PlanDuring' /><Value Type='Number'>" + during + "</Value></Eq></And><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Lookup'>" + userID + "</Value></Eq></And></Or></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                item = listItems[0];
                        }
                    }
                }
            });
            return item;
        }
        private bool  ListExits( )
        {
            bool isExits = false;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID ))
                    {
                        SPList parentList = spWeb.Lists.TryGetList(webObj.ListName);
                        if (parentList == null)
                        {
                            isExits = false;
                             
                        }
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
