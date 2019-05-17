using System;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace VATaskWebPart.CreateScheduleByTask
{
    //通过创建好的任务自动生成一周的日程
    /// <summary>
    /// 按开始日期、优先级进行排序；预估工作量按人平均
    /// 工作时间上午9~11，下午13:30~ 15:30
    /// </summary>
    public partial class CreateScheduleByTaskUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnImport.Click += BtnImport_Click;
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            Guid siteID = SPContext.Current.Site.ID;
            Guid webID = SPContext.Current.Web.ID;
            CreateSchedule(siteID, webID, webObj.SchedulelList);//日程列表
        }
        #region 方法
        public CreateScheduleByTask webObj { get; set; }

        //创建日程
        /// <summary>
        /// 工作时间9:00，下午13:30
        /// </summary>
        private void CreateSchedule(Guid siteID, Guid webID, string listName)
        {
            string[] users = webObj.AssignedTo.Split(';');// 用户列表
            SPListItemCollection allTasks = GetTasks();
            SPItem scheduleItem = null;
            DateTime beginTime = DateTime.Now;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite mySite = new SPSite(siteID))
                {
                    using (SPWeb myWeb = mySite.AllWebs[webID])
                    {
                        //List<DateTime > allSchedules = new List<DateTime >();
                        Dictionary<DateTime, int> allSchedules = new Dictionary<DateTime, int>();

                        DateTime dStart;
                        DateTime dBegin;

                        myWeb.AllowUnsafeUpdates = true;
                        SPList scheduleList = myWeb.Lists[listName];

                        foreach (SPListItem item in allTasks)
                        {
                            if (item["EstimatedDuring"] == null) continue;//预做工作为空，则继续遍历
                            dStart = (DateTime)item["StartDate"]; //任务的开始日期
                            dBegin = dStart;
                            int hours = int.Parse (item["EstimatedDuring"].ToString ());//预估工作量，截止日期、开始日期
                            int day = ((DateTime)item["DueDate"]).Subtract((DateTime)item["StartDate"]).Days + 1;//天数，开始日期与截止日期的差值
                            hours = hours / (day * users.Length);//每个的工作量，按人数平分
                            int tmpHours=hours ;
                            int totalHours = hours;//总学时
                            #region 同一天的任务 
                            while (dBegin <= (DateTime)item["DueDate"])//先分同一个任务，多天同时进行日程创建
                            {
                                tmpHours = totalHours ;//新的一天重新分配
                                while (tmpHours > 0)//同一天多次分配
                                {
                                    if (tmpHours > 2)
                                        hours = 2;
                                    else
                                        hours = tmpHours;
                                    dStart = new DateTime(dBegin.Year, dBegin.Month, dBegin.Day, 9, 0, 0);
                                    if (!allSchedules.ContainsKey(dStart))//上午的日程划分,如果超过两小时
                                    {
                                            allSchedules.Add(dStart, hours);
                                    }
                                    else
                                    {
                                        dStart = new DateTime(dBegin.Year, dBegin.Month, dBegin.Day, 13, 30, 0);//下午的日程划分
                                        if (!allSchedules.ContainsKey(dStart))
                                            allSchedules.Add(dStart, hours);
                                    }
                                    #region 逐个用户进行分
                                    foreach (string user in users)
                                    {
                                        int userID = GetAuthorID(user, myWeb);//用户ID，按用户进行遍历 
                                        scheduleItem = GetItemByPlanDate(dStart.ToString(), hours * 60, userID, item.ID);
                                        if (scheduleItem == null)
                                        {
                                            //创建日程
                                            scheduleItem = scheduleList.AddItem();
                                            scheduleItem["Task"] = item.ID;
                                            scheduleItem["PlanStart"] = dStart;
                                            scheduleItem["PlannedDuring"] = hours * 60;
                                            scheduleItem["AssignedTo"] = userID;
                                            scheduleItem.Update();
                                        }
                                    }
                                    #endregion
                                    tmpHours = tmpHours - 2;
                                }
                                dBegin = dBegin.AddDays(1);
                            }
                            #endregion
                        }
                        myWeb.AllowUnsafeUpdates = false;
                    }
                }
            });
            lblMsg.Text = "生成完成！";
        }
        private int GetAuthorID(string account, SPWeb myWeb)
        {
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
        #endregion
        #region 获取任务
        /// <summary>
        /// 获取一周的数据，按时间和优先级排序
        /// 开始日期作为查询条件，只选原子任务时行分配，排除操作等于所有的
        /// </summary>
        /// <returns></returns>
        private SPListItemCollection GetTasks()
        {
            string fldName = "Task";//查阅项的字段名称，通过这个字段找父表的数据 
            SPListItemCollection items = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.SchedulelList);
                        if (spList != null)
                        {
                            SPFieldLookup task = spList.Fields.GetFieldByInternalName(fldName) as SPFieldLookup;
                            SPList taskList = spWeb.Lists[new Guid(task.LookupList)];
                            SPQuery qry = new SPQuery();
                            DateTime dtStart = webObj.StartDate;
                            qry.ViewFields = "<FieldRef Name='Title' /><FieldRef Name = 'StartDate' /><FieldRef Name = 'DueDate' /><FieldRef Name = 'EstimatedDuring' /><FieldRef Name = 'Action' />< FieldRef Name = 'AssignedTo' />";
                            qry.Query = @"<Where><And><Neq>
         <FieldRef Name='Action' />
         <Value Type='Text'>所有</Value>
      </Neq><Or><And><Geq><FieldRef Name='StartDate' /><Value Type='DateTime'>" 
+ SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(dtStart.ToShortDateString())) 
+ "</Value></Geq><Lt><FieldRef Name = 'StartDate' /><Value Type = 'DateTime'> " 
+ SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(dtStart.AddDays(webObj.days).ToShortDateString()))
+ " </Value></Lt></And><And><Geq><FieldRef Name='DueDate' /><Value Type='DateTime'>"
+ SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(dtStart.ToShortDateString()))
+ "</Value></Geq><Lt><FieldRef Name = 'DueDate' /><Value Type = 'DateTime'> "
+ SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(dtStart.AddDays(webObj.days).ToShortDateString()))
+ " </Value></Lt></And></Or></And></Where><OrderBy><FieldRef Name='StartDate' /><FieldRef Name='Priority' /></OrderBy>";
                            SPListItemCollection listItems = taskList.GetItems(qry);
                            if (listItems.Count > 0)
                                items = listItems;
                        }
                    }
                }
            });
            return items;
        }
        #endregion
        #region 是否已经写入日程
        /// <summary>
        /// 通过计划开始和计划时长判断当前用户是否已经被分配日程
        /// 条件：任务ID，计划开始(包括时间)，计划时长，分配者
        /// </summary>
        /// <returns></returns>
        private SPListItem GetItemByPlanDate(string planDate, int during, int userID, int taskID)
        {
            SPListItem item = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.SchedulelList);//查找日程列表
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            planDate = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(planDate));
                            qry.ViewFields = "<FieldRef Name='Title' /><FieldRef Name='PlanStart' /><FieldRef Name='PlannedDuring' /><FieldRef Name='Task' /><FieldRef Name='AssignedTo' />";
                            qry.Query = @" <Where><And><And><And><Eq><FieldRef Name='Task' LookupId='True' /><Value Type='Lookup'>" 
+ taskID +"</Value></Eq><Eq>" +
              "<FieldRef Name= 'AssignedTo' LookupId = 'True' /><Value Type = 'Lookup'>" + userID + "</Value></Eq>" +
              "</And>" +
              "<Eq><FieldRef Name='PlanStart' />" +
               "<Value Type= 'DateTime' IncludeTimeValue = 'TRUE'>" + planDate + "</Value></Eq>" +
                "</And><Eq><FieldRef Name='PlannedDuring' /><Value Type='Number'>" + during + "</Value></Eq>" +
                "</And></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE'/></OrderBy>";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                item = listItems[0];
                        }
                    }
                }
            });
            return item;
        }
        #endregion
    }
}
