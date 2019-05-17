using Microsoft.SharePoint;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace RoutinePlan.VisualWebPart1
{
    public partial class VisualWebPart1UserControl : UserControl
    {
        public VisualWebPart1 webObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SPContext.Current.Site.OpenWeb().CurrentUser == null)
            {
                AppraiseDiv.Visible = false;
            }
            else
            {
                AppraiseDiv.Visible = true;
                if (!IsPostBack)
                {
                    string rPlanList = webObj.RoutinePlanList;
                    string siteUrl = webObj.SiteUrl;
                    int colCount = webObj.ColCount;
                    int minValue = webObj.MinValue;
                    int maxValue = webObj.MaxValue;
                    lbDaysSpan.Text = "（最少"+minValue+"天，最多"+maxValue+"天）";
                    BindRoutinePlan(rPlanList, siteUrl,colCount);
                    for (int i = 0; i < cblPlans.Items.Count; i++)
                    {
                        cblPlans.Items[i].Selected = true;
                    }
                }
            }
        }


        protected void btnMakePlans_Click(object sender, EventArgs e)
        {
            string rPlanList = webObj.RoutinePlanList;
            string siteUrl = webObj.SiteUrl;
            string planList = webObj.PlanList;
            string logList = webObj.LogList;
            int minValue= webObj.MinValue;
            int maxValue = webObj.MaxValue;
            if (IsIntNum(tbDays.Text.Trim()))
            {
                int days = int.Parse(tbDays.Text.Trim());
                if (days>= minValue && days<= maxValue)
                {
                    MakeRoutinePlan(logList, rPlanList, siteUrl, planList, days);
                }
                else
                {
                    lbErr.Text = "请输入生成天数,天数为 " + minValue+" - "+maxValue+" 之内的整数";
                }
            }
            else
            {
                lbErr.Text = "请输入生成天数,天数为 " + minValue + " - " + maxValue + " 之内的整数";
            }
        }

        public static bool IsIntNum(string str)
        {
            bool ismatch;
            if (string.IsNullOrEmpty(str))
            {
                ismatch = false;
            }
            else
            {
                Regex reg= new Regex(@"^[-]?[1-9]{1}\d*$|^[0]{1}$");
                ismatch = reg.IsMatch(str);
            }
            return ismatch;
        }

        /// <summary>
        /// 根据需要从计算所得的某天起生成计算所得天数的例行计划，并填入planList列表
        /// </summary>
        /// <param name="LogList">执行历史列表</param>
        /// <param name="rPlanList">例行计划列表</param>
        /// <param name="siteUrl">网站地址</param>
        /// <param name="planList">生成计划保存的列表</param>
        /// <param name="days">用户设置的天数</param>
        private void MakeRoutinePlan(string LogList, string rPlanList, string siteUrl, string planList, int days)
        {
            SPUser currentUser = SPContext.Current.Site.OpenWeb().CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    int k = 0;
                    using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            spWeb.AllowUnsafeUpdates = true;

                            SPList spList = spWeb.Lists.TryGetList(rPlanList);
                            if (spList != null)
                            {
                                SPListItemCollection rPlanListItems = spList.GetItems();
                                if (rPlanListItems.Count > 0)
                                {
                                    Dictionary<int, string> dict = GetChecked(cblPlans);
                                    foreach (KeyValuePair<int, string> kv in dict)
                                    {
                                        int planId = kv.Key;
                                        string planTitle = kv.Value;
                                        SPListItem rPlanListItem = rPlanListItems.GetItemById(planId);
                                        ArrayList leftdays=SetLog(spWeb, LogList, days, currentUser, planId,planTitle);
                                        if (cbWeekEnd.Checked)//跳过周末
                                        {
                                            ArrayList workDays = QueryWeekDays(leftdays);
                                            writeToPlan(spWeb, planList, workDays, currentUser, rPlanListItem);
                                        }
                                        else
                                        {
                                            writeToPlan(spWeb, planList, leftdays, currentUser, rPlanListItem);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                lbErr.Text = "“" + rPlanList + "”列表不存在！";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lbErr.Text = "行号 "+ex.StackTrace+":"+ex.ToString();
                }
            });
        }

        private void writeToPlan(SPWeb spWeb, string planList, ArrayList leftDays, SPUser currentUser, SPListItem rPlanListItem)
        {
            if (leftDays.Count > 0)
            {
                try
                {
                    SPList pList = spWeb.Lists.TryGetList(planList);
                    if (pList != null)
                    {
                        for (int j = 0; j < leftDays.Count; j++)
                        {
                            SPListItem pitem = pList.AddItem();
                            pitem["对象"] = rPlanListItem["标题"];
                            DateTime dtStart = (DateTime)rPlanListItem["计划开始"];
                            pitem["计划开始"] = ((DateTime)leftDays[j]).Date + dtStart.TimeOfDay;
                            pitem["计划时长"] = rPlanListItem["计划时长"];

                            pitem["活动操作"] = rPlanListItem["操作"];
                            pitem["创建者"] = currentUser.ID;
                            pitem["修改者"] = currentUser.ID;
                            pitem.Update();
                        }
                    }
                    else
                    {
                        lbErr.Text = "“" + planList + "”列表不存在！";
                    }
                }
                catch (Exception ex)
                {

                    lbErr.Text = ex.ToString();
                }
            }
            else
            {
                lbErr.Text = "你所设置的天数内，没有例行计划要生成！";
            }
        }

        /// <summary>
        /// 查询执行历史，判断是否需要生成新数据，若生成，则返回需要生成的天数，否则返回0
        /// </summary>
        /// <param name="spWeb">网站</param>
        /// <param name="LogList">历史列表</param>
        /// <param name="days">用户填写的天数</param>
        /// <param name="currentUser">当前用户</param>
        /// <param name="planId">要执行的例行计划ID</param>
        /// <param name="planTitle">要执行的例行计划标题</param>
        /// <returns></returns>
        private ArrayList SetLog(SPWeb spWeb, string LogList, int days, SPUser currentUser, int planId, string planTitle)
        {
            ArrayList alist = new ArrayList();
            spWeb.AllowUnsafeUpdates = true;
            try
            {
                SPList logList = spWeb.Lists.TryGetList(LogList);
                if (logList != null)
                {
                    string isWeekEnd = "包含周末";
                    if (cbWeekEnd.Checked)//跳过周末
                    {
                        isWeekEnd = "跳过周末";
                    }
                    SPQuery qry = new SPQuery();

                    qry.Query = "<Where><And><Eq><FieldRef Name='PlanID'/><Value Type='Number'>" + planId + "</Value></Eq><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Integer'>" + currentUser.ID + "</Value></Eq></And></Where>";
                    //查找当前用户执行planId所对应的例行计划的历史记录
                    SPListItemCollection logListItems = logList.GetItems(qry);
                    DateTime dtNow = DateTime.Now;
                    if (logListItems.Count > 0)//记录已找到
                    {
                        SPListItem logItem = logListItems[0];
                        DateTime dtEnd = (DateTime)logItem["截止日期"];
                        int n = GetNumberInt(logItem["标题"].ToString());
                        if (dtEnd.Date > dtNow.Date)
                        {
                            int leftdays = days - DateDiff(dtNow, dtEnd) - 1;
                            if (leftdays > 0)
                            {
                                n = n + 1;
                                logItem["标题"] = currentUser.Name + "第" + n + "次生成“"+planTitle+"”计划";
                                logItem["开始日期"] = dtEnd.AddDays(1).Date;
                                logItem["截止日期"] = dtEnd.AddDays(leftdays).Date;
                                logItem["历史记录"] = logItem["历史记录"] + "第" + n + "次（" + dtNow + "）：" + dtEnd.AddDays(1).ToShortDateString() + "-" + dtEnd.AddDays(leftdays).ToShortDateString() + "（" + isWeekEnd + "）;";
                                //logItem["例行计划ID"] = planId;
                                logItem.Update();
                                for (int i = 1; i <= leftdays; i++)
                                {
                                    alist.Add(dtEnd.AddDays(i).Date);
                                }
                                lbErr.Text = "本次执行共生成了从 " + dtEnd.AddDays(1).ToShortDateString() + "起， " + leftdays + " 天（"+isWeekEnd+"）的例行计划！";
                            }
                            else
                            {
                                lbErr.Text = "从今天起，" + days + "天内的例行计划已生成，无需重复执行！";
                            }
                        }
                        else
                        {
                            n = n + 1;
                            logItem["标题"] = currentUser.Name + "第" + n + "次生成“" + planTitle + "”计划";
                            logItem["开始日期"] = dtNow;
                            logItem["截止日期"] = dtNow.AddDays(days - 1);
                            logItem["历史记录"] = logItem["历史记录"] + "第" + n + "次(" + dtNow + ")：" + dtNow.ToShortDateString() + "-" + dtNow.AddDays(days - 1).ToShortDateString() + "（" + isWeekEnd + "）;";
                            //logItem["例行计划ID"] = planId;
                            logItem.Update();
                            for (int i = 0; i < days; i++)
                            {
                                alist.Add(dtNow.AddDays(i).Date);
                            }
                            lbErr.Text = "本次执行共生成了从今天起， " + days + " 天（" + isWeekEnd + "）的例行计划！";
                        }
                    }
                    else//记录未找到
                    {
                        SPListItem logItem = logList.AddItem();
                        logItem["标题"] = currentUser.Name + "第1次生成“" + planTitle + "”计划";
                        logItem["开始日期"] = dtNow;
                        logItem["截止日期"] = dtNow.AddDays(days - 1);
                        logItem["历史记录"] = "第1次(" + dtNow + ")：" + dtNow.ToShortDateString() + "-" + dtNow.AddDays(days - 1).ToShortDateString() + "（" + isWeekEnd + "）;";
                        logItem["例行计划ID"] = planId;
                        logItem["创建者"] = currentUser.ID;
                        logItem["修改者"] = currentUser.ID;
                        logItem.Update();
                        for (int i = 0; i < days; i++)
                        {
                            alist.Add(dtNow.AddDays(i).Date);
                        }
                        lbErr.Text = "本次执行共生成了从今天起， " + days + " 天（" + isWeekEnd + "）的例行计划！";
                    }
                }
                else
                {
                    lbErr.Text = "“" + LogList + "”列表不存在！";
                }
            }
            catch (Exception ex)
            {
                lbErr.Text = ex.ToString();
            }
            return alist;
        }

        private ArrayList QueryWeekDays(ArrayList sList)
        {
            ArrayList aList = new ArrayList();
            foreach (var item in sList)
            {
                DateTime dt = (DateTime)item;
                if (dt.DayOfWeek.ToString()!= "Saturday" && dt.DayOfWeek.ToString()!="Sunday")
                {
                    aList.Add(item);
                }
            }
            return aList;
        }

        private void BindRoutinePlan(string rPlanList, string siteUrl,int colCount)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            if (rPlanList != "")
                            {

                                SPList spList = spWeb.Lists.TryGetList(rPlanList);
                                if (spList != null)
                                {
                                    SPListItemCollection items = spList.GetItems();
                                    cblPlans.DataTextField = "Title";
                                    cblPlans.DataValueField = "ID";
                                    cblPlans.DataSource = items;
                                    cblPlans.DataBind();
                                    cblPlans.RepeatColumns = colCount;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    lbErr.Text = ex.ToString();
                }
            });
        }

        #region 判断工作日
        //获取当前周几

        private string _strWorkingDayAM = "08:30";//工作时间上午08:00
        private string _strWorkingDayPM = "17:30";
        private string _strRestDay = "6,7";//周几休息日 周六周日为 6,7

        private TimeSpan dspWorkingDayAM;//工作时间上午08:00
        private TimeSpan dspWorkingDayPM;
        /// <summary>
        /// 判断一个时间是星期几
        /// </summary>
        /// <param name="dt">指定时间</param>
        /// <returns></returns>
        private string m_GetDateTimeWeek(DateTime dt)
        {
            string strWeek = dt.DayOfWeek.ToString();
            switch (strWeek)
            {
                case "Monday":
                    {
                        return "1";
                    }
                case "Tuesday":
                    {
                        return "2";
                    }
                case "Wednesday":
                    {
                        return "3";
                    }
                case "Thursday":
                    {
                        return "4";
                    }
                case "Friday":
                    {
                        return "5";
                    }
                case "Saturday":
                    {
                        return "6";
                    }
                case "Sunday":
                    {
                        return "7";
                    }
            }
            return "0";
        }


        /// <summary>
        /// 判断是否在工作日内
        /// </summary>
        /// <param name="dt">指定时间</param>
        /// <returns></returns>
        private bool m_IsWorkingDay(DateTime dt)
        {
            string strWeekNow = this.m_GetDateTimeWeek(dt);//当前周几
                                                    ////判断是否有休息日
            string[] RestDay = _strRestDay.Split(',');
            if (RestDay.Contains(strWeekNow))
            {
                return false;
            }
            //判断当前时间是否在工作时间段内

            dspWorkingDayAM = DateTime.Parse(_strWorkingDayAM).TimeOfDay;
            dspWorkingDayPM = DateTime.Parse(_strWorkingDayPM).TimeOfDay;

            TimeSpan dspDT = dt.TimeOfDay;
            if (dspDT > dspWorkingDayAM && dspDT < dspWorkingDayPM)
            {
                return true;
            }
            return false;
        }
        //初始化默认值
        private void m_InitWorkingDay()
        {
            dspWorkingDayAM = DateTime.Parse(_strWorkingDayAM).TimeOfDay;
            dspWorkingDayPM = DateTime.Parse(_strWorkingDayPM).TimeOfDay;

        }
        #endregion


        /// <summary>
        /// 遍历多选框将所有选中的项的ID和值存入键值对
        /// </summary>
        /// <param name="checkList">多选框控件ID</param>
        /// <returns>Dictionary<int, string></returns>
        public static Dictionary<int, string> GetChecked(CheckBoxList checkList)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            for (int i = 0; i < checkList.Items.Count; i++)
            {
                if (checkList.Items[i].Selected)
                {
                    dict.Add(int.Parse(checkList.Items[i].Value), checkList.Items[i].Text);
                }
            }
            return dict;
        }
        protected void rblistCycle_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rbValue = rblistCycle.SelectedValue;
            switch (rbValue)
            {
                case "rbDay":
                    DayModel.Visible = true;
                    WeekModel.Visible = false;
                    MonthModel.Visible = false;
                    YearModel.Visible = false;
                    break;
                case "rbWeek":
                    DayModel.Visible = false;
                    WeekModel.Visible = true;
                    MonthModel.Visible = false;
                    YearModel.Visible = false;
                    break;
                case "rbMonth":
                    DayModel.Visible = false;
                    WeekModel.Visible = false;
                    MonthModel.Visible = true;
                    YearModel.Visible = false;
                    break;
                case "rbYear":
                    DayModel.Visible = false;
                    MonthModel.Visible = false;
                    WeekModel.Visible = false;
                    YearModel.Visible = true;
                    break;
                default:
                    DayModel.Visible = true;
                    WeekModel.Visible = false;
                    MonthModel.Visible = false;
                    YearModel.Visible = false;
                    break;
            }
        }

        protected void rbEndDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEndDate.Checked)
            {
                rbEndTimes.Checked = false;
                rbNoEndDate.Checked = false;
            }
        }

        protected void rbEndTimes_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEndTimes.Checked)
            {
                rbEndDate.Checked = false;
                rbNoEndDate.Checked = false;
            }
        }

        protected void rbNoEndDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNoEndDate.Checked)
            {
                rbEndDate.Checked = false;
                rbEndTimes.Checked = false;
            }
        }

        protected void rbDayPerNDay_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDayPerNDay.Checked)
            {
                rbDayPerWorkDay.Checked = false;
            }
        }

        protected void rbDayPerWorkDay_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDayPerWorkDay.Checked)
            {
                rbDayPerNDay.Checked = false;
            }
        }

        protected void rbMonth1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMonth1.Checked)
            {
                rbMonth2.Checked = false;
            }
        }

        protected void rbMonth2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMonth2.Checked)
            {
                rbMonth1.Checked = false;
            }
        }

        protected void rbYear1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbYear1.Checked)
            {
                rbYear2.Checked = false;
            }
        }

        protected void rbYear2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbYear2.Checked)
            {
                rbYear1.Checked = false;
            }
        }

        /// <summary>
        /// 计算开始时间和结束时间之间相差的天数
        /// </summary>
        /// <param name="dateStart">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns></returns>
        private static int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());

            TimeSpan sp = end.Subtract(start);

            return sp.Days;
        }
        /// <summary>
        /// 获取字符串中的整数数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>数字</returns>
        public static int GetNumberInt(string str)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(str))
            {
                // 正则表达式剔除非数字字符（不包含小数点.）
                str = Regex.Replace(str, @"[^\d.\d]", "");
                // 如果是数字，则转换为decimal类型
                if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
                {
                    result = int.Parse(str);
                }
            }
            return result;
        }


    }
}
