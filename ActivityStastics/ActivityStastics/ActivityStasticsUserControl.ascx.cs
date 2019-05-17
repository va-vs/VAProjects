using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace ActivityStastics.ActivityStastics
{
    public partial class ActivityStasticsUserControl : UserControl
    {
        public ActivityStastics webObj { get; set; }
        #region 事件
        
       
        protected void Page_Load(object sender, EventArgs e)
        {
            string planList = webObj.PlanList;
            string siteUrl = webObj.SiteUrl;
            string actionList = webObj.ActionList;
            string actionTypeList = webObj.ActionTypeList;
            string resultList = webObj.ResultList;
            if (!IsPostBack)
            {
                try
                {

                    string typeName = "项目";
                    BindDDL(actionTypeList, ddlTypes, typeName, siteUrl);

                    SPUser user = SPContext.Current.Web.CurrentUser;

                    //获取元数据
                    DataTable dtCombined = ComBinTypeAndResultsToRecords("元数据", planList, siteUrl, actionList, resultList);
                    Session["元数据表"] = dtCombined;
                    DataTable dtTypes = GetTypesTable(actionTypeList, siteUrl);
                    Session["操作类别表"] = dtTypes;

                    if (dtCombined != null)
                    {
                        DataTable dtComputed = DTComputeByTypeInPeriods(dtCombined, dtTypes);
                        if (dtComputed == null)
                        {
                            lbErr.Text = "活动类别雷达图无数据，不显示";
                            chartRadar.Visible = false;
                        }
                        else
                        {
                            RadarBind(dtComputed, "今日活动类别雷达图");
                        }

                        DataTable dtDates = GetDates(7, "PlanStart");
                        string[] groupFields = new string[] { "PlanStart", "Start" };
                        string[] computeFields = new string[] { "PlanDuring", "During" };
                        DataTable dtResult = DTCompute(dtCombined, "项目", groupFields, computeFields, dtDates, "日期");
                        //string[] xfields = new string[] { "PlanStart","Start" };
                        //string[] yfields = new string[] { "PlanDuring" , "PlanDuring" };
                        ChartBindWeek(dtResult, Chart1, "近一周每日“项目”活动时长变化趋势", "日期", computeFields, SeriesChartType.Line);
                    }
                    else
                    {
                        chartRadar.Visible = false;
                        lbErr.Text = "无数据";
                    }
                }
                catch (Exception ex)
                {
                    lbErr.Text = ex.ToString();
                }
            }                        
        }

        private DataTable GetTypesTable(string actionTypeList, string siteUrl)
        {
            DataTable dtReturn = new DataTable();
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        SPList spList = spWeb.Lists.TryGetList(actionTypeList);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            qry.ViewFields = "<FieldRef Name='Title' />";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                            {
                                dtReturn= listItems.GetDataTable();
                            }
                            else
                            {
                                dtReturn= null;
                            }
                        }
                        else
                        {
                            dtReturn= null;
                        }
                    }
                }
            });
            return dtReturn;
        }

        private DataTable DTComputeByTypeInPeriods(DataTable dtSource,DataTable dtTypes)
        {
            DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add("类别", typeof(string));
            dtReturn.Columns.Add("计划时长", typeof(double));
            dtReturn.Columns.Add("实际时长", typeof(double));
            string filter = GetTimeFilter("PlanStart");

            if (filter=="")
            {
                dtReturn = null;
                lbErr.Text = "请注意选择正确的筛选日期范围！";
            }
            else
            {
                for (int i = 0; i < dtTypes.Rows.Count; i++)
                {

                    filter = filter + " and TypeName='" + dtTypes.Rows[i][0].ToString() + "'";
                    DataRow dr = dtReturn.NewRow();
                    dr[0] = dtTypes.Rows[i][0];
                    object result = dtSource.Compute("sum(PlanDuring)", filter);
                    result = result == null ? 0 : result;
                    dr[1] = result;

                    filter = GetTimeFilter("Start") + " and TypeName='" + dtTypes.Rows[i][0].ToString() + "'";
                    result = dtSource.Compute("sum(During)", filter);
                    result = result == null ? 0 : result;
                    dr[2] = result;

                    dtReturn.Rows.Add(dr);
                }
            }            
            return dtReturn;
        }

        private void RadarBind(DataTable dt,string chartTitle)
        {
            //标题设置
            chartRadar.Titles.Clear();
            chartRadar.Titles.Add(chartTitle);

            //数据绑定
            //Color[] bdcolor = new Color[2] {Color.Red,Color.Blue };
            //Color[] sColor = new Color[2] { Color.FromArgb(50, 255, 0, 0), Color.FromArgb(50, 0, 0, 255) };
            for (int i = 0; i < dt.Columns.Count - 1; i++)//在前台设置好Series的个数与yFields个数相同，并设定好每个Series的样式
            {
                string xField = dt.Columns[0].ColumnName;
                string yField = dt.Columns[i + 1].ColumnName;
                chartRadar.Series[i].Points.DataBind(dt.DefaultView, xField, yField, string.Format("LegendText={0},YValues={1},ToolTip={0}", xField, yField));
                chartRadar.Series[i].ToolTip = chartRadar.Series[i].Name + " #VAL \r\n #AXISLABEL";
                //chartRadar.Series[i].ChartType = SeriesChartType.Radar;
                //chartRadar.Series[i].BorderWidth = 2;
                //chartRadar.Series[i].ShadowColor = Color.LightYellow;
                //chartRadar.Series[i].BorderColor = bdcolor[i];
                //chartRadar.Series[i].Color = sColor[i];
            }
            

            //背景色设置
            chartRadar.ChartAreas[0].ShadowColor = Color.Transparent;
            chartRadar.ChartAreas[0].BackColor = Color.FromArgb(209, 237, 254);//该处设置为了由天蓝到白色的逐渐变化
            chartRadar.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
            chartRadar.ChartAreas[0].BackSecondaryColor = Color.White;

            //中间X,Y线条的颜色设置
            chartRadar.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartRadar.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);

            //3D设置
            chartRadar.ChartAreas[0].Area3DStyle.Enable3D = true;

            
            chartRadar.DataBind();
        }

        /// <summary>
        /// 查询制定用户的数据,若用户为空,则返回所有用户的数据
        /// </summary>
        /// <param name="user">指定用户SPUser</param>
        /// <param name="dateSpan">日期区间</param>
        /// <param name="planList">数据列表名称</param>
        /// <param name="siteUrl">数据列表所在网站</param>

        /// <returns></returns>
        private DataTable GetDataBySPUserandDateSpan(SPUser user,DateTime[] dateSpan, string planList, string siteUrl)
        {
            DataTable dtReturn = new DataTable();
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        SPList spList = spWeb.Lists.TryGetList(planList);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();                           

                            qry.Query = @"<Where><IsNotNull><FieldRef Name='CustAction'/></IsNotNull></Where>";
                            if (user != null)//当前网站用户已登录
                            {
                                qry.Query = @"<Where><And><IsNotNull><FieldRef Name='CustAction' /></IsNotNull><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Integer'>" + user.ID + "</Value></Eq></And></Where>";
                            }

                            SPListItemCollection items = spList.GetItems(qry);
                            if (items.Count > 0)
                            {
                                dtReturn = items.GetDataTable();
                            }
                            else
                            {
                                dtReturn = null;
                            }
                        }

                    }
                }

            });
            return dtReturn;
        }

        
        private DataTable dtComputeByCol(string computeType,DataTable dtSource)
        {
            DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add("");
            if (computeType=="类别")
            {
                dtReturn.Columns.Add("");
            }
            else
            {

            }
            return dtReturn;
        }

        private DataTable ComBinTypeAndResultsToRecords(string dtTitle,string pList ,string siteUrl,string actionList,string resultList)
        {
            DataTable dtCombined = CreatCombinedDataTable(dtTitle);
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        SPList spList = spWeb.Lists.TryGetList(pList);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();

                            SPUser user = SPContext.Current.Web.CurrentUser;

                            qry.Query = @"<Where><IsNotNull><FieldRef Name='CustAction' /></IsNotNull></Where>";
                            if (user != null)//当前网站用户已登录
                            {
                                //gpExp += " and Author='" + user.Name + "'";
                                qry.Query = @"<Where><And><IsNotNull><FieldRef Name='CustAction' /></IsNotNull><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Integer'>"+user.ID+"</Value></Eq></And></Where>";
                            }
                            //qry.ViewFields = @"<FieldRef Name='ID' />
                            //                    <FieldRef Name='Title' />
                            //                    <FieldRef Name='Author' />
                            //                    <FieldRef Name='Created' />
                            //                    <FieldRef Name='PlanDate' />
                            //                    <FieldRef Name='PlanDuring' />
                            //                    <FieldRef Name='ActualDate' />
                            //                    <FieldRef Name='ActualDuring' />
                            //                    <FieldRef Name='CustAction' />";
                            SPListItemCollection items = spList.GetItems(qry);
                            if (items.Count>0)
                            {
                                for (int i = 0; i < items.Count; i++)
                                {
                                    DataRow dr = dtCombined.NewRow();
                                    dr[0] = items[i]["ID"];//ID 0
                                    dr[1] = items[i]["Title"];//标题 1
                                    if (items[i]["CustAction"]!=null)
                                    {
                                        int actionId = int.Parse(items[i]["CustAction"].ToString().Split(';')[0]);
                                        string[] value = GetTypeinActionListByID(actionId, siteUrl, actionList).Split(';');
                                        dr[2] = value[0];//类别Id 2
                                        dr[3] = value[1].TrimStart('#');//类别名称 3
                                    }

                                    //当计划开始时间为空时，以创建时间为准
                                    if (items[i]["PlanDate"] != null)
                                    {
                                        dr[4] = ((DateTime)items[i]["PlanDate"]).Date;//计划开始 4
                                    }
                                    else
                                    {
                                        dr[4] = ((DateTime)items[i]["Created"]).Date;//创建时间 
                                    }
                                    //当实际时长为空时，记为0
                                    dr[5] = items[i]["PlanDuring"] == null ? 0 : items[i]["PlanDuring"];//计划时长 5

                                    //当实际开始时间为空时，以计划开始时间为准
                                    if (items[i]["ActualDate"] != null)
                                    {
                                        dr[6] = ((DateTime)items[i]["ActualDate"]).Date;//实际开始 6
                                    }
                                    else
                                    {
                                        dr[6] = dr[4];//实际开始=计划开始
                                    }

                                    //当实际时长为空时，记为0
                                    dr[7] = items[i]["ActualDuring"] == null ? 0 : items[i]["ActualDuring"];//实际时长 7
                                    if (items[i]["ID"] != null)
                                    {
                                        string Id = items[i]["ID"].ToString();
                                        int activiId = int.Parse(Id);
                                        int result = GetActivityResults(activiId, siteUrl, resultList);
                                        dr[8] = result;//活动结果数 8
                                    }
                                    else
                                    {
                                        dr[8] = 0;
                                    }
                                    string[] author = items[i]["Author"].ToString().Split(';');
                                    dr[9] = author[0];//用户ID 9
                                    dr[10] = author[1].TrimStart('#');//用户名 10
                                    dtCombined.Rows.Add(dr);
                                }                            
                            }
                            else
                            {
                                dtCombined = null;
                            }
                        }

                    }
                }

            });

            return dtCombined;
        }


        /// <summary>
        /// DataTable筛选部分行输出到新DataTable
        /// </summary>
        /// <param name="oldTable">原DataTable</param>
        /// <param name="filterExp">筛选条件</param>
        /// <returns>新DataTable</returns>
        private DataTable DTFilter(DataTable oldTable,string filterExp)
        {
            DataTable newTable = oldTable.Clone();// 克隆原来的DataTable结构
            DataRow[] drs = oldTable.Select(filterExp);// 从oldTable中查询符合条件的记录； 
            foreach (DataRow dr in drs)
            {
                newTable.Rows.Add(dr.ItemArray);// 将查询的结果添加到新的DataTable中；
            }
            return newTable;
        }
        private string GetTypeinActionListByID(int actionId,string siteUrl,string actionList)
        {
            string typeValue = "";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        SPList spList = spWeb.Lists.TryGetList(actionList);
                        if (spList != null)
                        {
                            SPListItem item = spList.GetItemById(actionId);
                            typeValue = item["Type"].ToString();
                        }
                           
                    }
                }

            });
            return typeValue;
        }

        protected void ddlTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string typeName = ddlTypes.SelectedItem.Text;
            string planList = webObj.PlanList;
            string siteUrl = webObj.SiteUrl;
            string actionList = webObj.ActionList;
            string actionTypeList = webObj.ActionTypeList;
            string resultList = webObj.ResultList;
            try
            {
                DataTable dt = GetDatas(planList, actionList, actionTypeList,resultList, siteUrl);
                SPUser user = SPContext.Current.Web.CurrentUser;

                //string gpExp = "AType='"+ typeName + "'";
                //if (user != null)//当前网站用户已登录
                //{
                //    gpExp += " and Author='" + user.Name + "'";
                //}
                //DataTable dtGroup = GroupDt("源数据表", dt, gpExp,siteUrl,resultList);
                DataTable dtCombined = ComBinTypeAndResultsToRecords("元数据", planList, siteUrl, actionList, resultList);
                //GridView1.DataSource = dtCombined;
                //GridView1.DataBind();

                if (dtCombined != null)
                {
                    //DataTable dtTypes = GetTypesTable(actionTypeList, siteUrl);

                    //DataTable dtComputed = DTComputeByTypeInPeriods(dtCombined, dtTypes);
                    //ChartBind(dtComputed, chartRadar, "今日活动雷达图", SeriesChartType.Radar);

                    DataTable dtDates = GetDates(7, "PlanStart");
                    string[] groupFields = new string[] { "PlanStart", "Start" };
                    string[] computeFields = new string[] { "PlanDuring", "During" };
                    DataTable dtResult = DTCompute(dtCombined, "项目", groupFields, computeFields, dtDates, "日期");
                    //string[] xfields = new string[] { "PlanStart","Start" };
                    //string[] yfields = new string[] { "PlanDuring" , "PlanDuring" };
                    ChartBindWeek(dtResult, Chart1, "近一周每日“" + typeName + "”活动时长变化趋势", "日期", computeFields, SeriesChartType.Line);
                }
                else
                {
                    chartRadar.Visible = false;
                    lbErr.Text = "无数据";
                }
                
            }
            catch (Exception ex)
            {
                lbErr.Text = ex.ToString();
            }
        }
        #endregion

        #region 方法

        //获得搜索条件
        public string GetTypeFilter()
        {
            string filter = " 1=1 ";
            if (ddlTypes.SelectedValue != "4")
            {
                filter += string.Format(" and questiontype='{0}'", ddlTypes.SelectedValue);
            }
            if (rbPeriodList.SelectedValue != "0")
            {
                filter += string.Format(" and productline='{0}'", rbPeriodList.SelectedValue);
            }
            return filter;
        }

        private string GetTimeFilter(string filterField)
        {
            string filter = "1=1";
            string periodValue = rbPeriodList.SelectedValue;
            DateTime nowDate = DateTime.Now.Date;
            DateTime timeStart = nowDate;
            DateTime timeEnd = nowDate;
            
            switch (periodValue)
            {
                case "0"://自定义时间,用户尚未选择起止日期，筛选条件为空
                    
                    if (DateSelectRight())
                    {
                        timeStart = dtStart.SelectedDate.Date;
                        timeEnd = dtEnd.SelectedDate.Date;
                        filter = filterField + ">='" + timeStart + "' and " + filterField + "<='" + timeEnd + "'";
                    }
                    else
                    {
                        filter = "";
                    }                    
                    break;
                case "2"://本周
                    timeStart = GetTimeStartByType("Week",nowDate);
                    timeEnd = GetTimeEndByType("Week",nowDate);
                    filter = filterField + ">='" + timeStart + "' and " + filterField + "<='" + timeEnd + "'";
                    break;
                case "3"://本月
                    timeStart = GetTimeStartByType("Month", nowDate);
                    timeEnd = GetTimeEndByType("Month", nowDate);
                    filter = filterField + ">='" + timeStart + "' and " + filterField + "<='" + timeEnd + "'";
                    break;
                default:                    
                    filter = filterField+" = '"+ nowDate+"'";
                    break;
            }
            return filter;
        }

        private bool DateSelectRight()
        {
            bool isRight = false;
            if (dtStart.SelectedDate == null || dtEnd.SelectedDate == null)
            {
                lbErr.Text = "尚未选择筛选的起始日期或截至日期";
            }
            else
            {
                DateTime dt1 = dtStart.SelectedDate;
                DateTime dt2 = dtEnd.SelectedDate;
                if (dt2<dt1)
                {
                    lbErr.Text = "您选择的起始日期不能晚于截至日期，请重新选择！";
                }
                else
                {
                    isRight = true;
                }
            }          
            
            return isRight;
        }

        #region 获取 本周、本月、本季度、本年 的开始时间或结束时间
        /// <summary>
        /// 获取结束时间
        /// </summary>
        /// <param name="TimeType">Week、Month、Season、Year</param>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public static DateTime GetTimeStartByType(string TimeType, DateTime nowDate)
        {
            switch (TimeType)
            {
                case "Week":
                    return nowDate.AddDays(-(int)nowDate.DayOfWeek + 1);
                case "Month":
                    return nowDate.AddDays(-nowDate.Day + 1);
                case "Season":
                    var time = nowDate.AddMonths(0 - ((nowDate.Month - 1) % 3));
                    return time.AddDays(-time.Day + 1);
                case "Year":
                    return nowDate.AddDays(-nowDate.DayOfYear + 1);
                default:
                    return nowDate;
            }
        }

        /// <summary>
        /// 获取结束时间
        /// </summary>
        /// <param name="TimeType">Week、Month、Season、Year</param>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public static DateTime GetTimeEndByType(string TimeType, DateTime nowDate)
        {
            switch (TimeType)
            {
                case "Week":
                    return nowDate.AddDays(7 - (int)nowDate.DayOfWeek);
                case "Month":
                    return nowDate.AddMonths(1).AddDays(-nowDate.AddMonths(1).Day + 1).AddDays(-1);
                case "Season":
                    var time = nowDate.AddMonths((3 - ((nowDate.Month - 1) % 3) - 1));
                    return time.AddMonths(1).AddDays(-time.AddMonths(1).Day + 1).AddDays(-1);
                case "Year":
                    var time2 = nowDate.AddYears(1);
                    return time2.AddDays(-time2.DayOfYear);
                default:
                    return nowDate;
            }
        }
        #endregion

        public void BindDDL(string typesList,DropDownList ddl,string selectedText,string siteUrl)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        SPList spList = spWeb.Lists.TryGetList(typesList);
                        if (spList != null)
                        {
                            DataTable dt = spList.GetItems().GetDataTable();
                            if (dt.Rows.Count > 0)
                            {
                                ddl.DataSource = dt;
                                ddl.DataTextField = "Title";
                                ddl.DataValueField = "ID";
                                ddl.DataBind();
                                ddl.Items.FindByText(selectedText).Selected = true;
                            }
                            //else
                            //{
                            //    ListItem item = new ListItem("无类别可选","0");
                            //    ddl.Items.Add(item);
                            //}
                        }
                       
                    }
                }
              
               
            });
        }
        private DataTable GetDates(int days, string field)
        {
            using (DataTable dtResult = new DataTable())
            {
                dtResult.Columns.Add(field,typeof(DateTime));
                DateTime dtNow = DateTime.Now.Date;
                for (int i = days-1; i >= 0; i--)
                {
                    dtResult.Rows.Add(new object[] { dtNow.AddDays(-i) });
                }
                return dtResult;
            }
        }

        /// <summary>
        /// 合计
        ///</summary>
        /// <param name="dtSource">元数据表</param>
        /// <param name="groupFields">分组列</param>
        /// <param name="computeFields">计算列</param>
        /// <param name="dtCompute">分组元数据</param>
        /// <returns></returns>
        public DataTable DTCompute(DataTable dtSource, string typeName,string[] groupFields, string[] computeFields, DataTable dtCompute,string dtComputeName)
        {
            DataTable Newdt = DTFilter(dtSource, "TypeName='" + typeName + "'");

            if (dtCompute == null)//未设定筛选数据表表
            {
                dtCompute = Newdt.DefaultView.ToTable(true, groupFields);
                dtCompute.TableName = dtComputeName;
            }
            
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add(dtComputeName,typeof(string));
            for (int k = 0; k < computeFields.Length; k++)//多个计算列
            {                
                dtResult.Columns.Add(computeFields[k]);
            }   
            for (int i = 0; i < dtCompute.Rows.Count; i++)
            {
                DataRow dr = dtResult.NewRow();
                string gpStr = string.Format("{0:m}",dtCompute.Rows[i][0]);
                dr[0] = gpStr;
                int objectLength = computeFields.Length;
                object[] sumObjects = new object[objectLength];
                for (int j = 0; j < objectLength; j++)
                {
                    string filter = groupFields[j] + " = '" + dtCompute.Rows[i][0] + "'";
                    object sumObject = Newdt.Compute("sum(" + computeFields[j] + ")", filter);
                    if (sumObject == DBNull.Value)
                    {
                        sumObject = 0;
                    }
                    dr[j + 1] = sumObject;
                }
                dtResult.Rows.Add(dr);
            }
            return dtResult;
        }


        /// <summary>
        /// 获取指定字段不重复项，返回List
        /// </summary>
        /// <param name="dataTable">元数据表</param>
        /// <param name="distinctCol">指定字段名</param>
        /// <returns></returns>
        public List<string> GetDistinctColValuesFromDataTable(DataTable dataTable, string distinctCol)
        {
            DataView dv = dataTable.DefaultView;
            dataTable = dv.ToTable(true, distinctCol);
            List<string> dColValues = new List<string>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dColValues.Add(dataTable.Rows[i][0].ToString());
            }
            return dColValues;
        }

        private DataTable CreatCombinedDataTable(string dtTitle)
        {
            DataTable dtReturn = new DataTable(dtTitle);//共11列
            dtReturn.Columns.Add("ID", typeof(string));//ID 0
            dtReturn.Columns.Add("Title", typeof(string));//标题 1
            dtReturn.Columns.Add("TypeID", typeof(string));//类别Id 2
            dtReturn.Columns.Add("TypeName", typeof(string));//类别名称 3 
            dtReturn.Columns.Add("PlanStart", typeof(DateTime));//计划开始 4
            dtReturn.Columns.Add("PlanDuring", typeof(float));//计划时长 5
            dtReturn.Columns.Add("Start", typeof(DateTime));//实际开始 6
            dtReturn.Columns.Add("During", typeof(float));//实际时长 7
            dtReturn.Columns.Add("Results", typeof(int));//结果数量 8
            dtReturn.Columns.Add("UserID",typeof(string));//用户ID 9
            dtReturn.Columns.Add("UserName", typeof(string));//用户名 10
            return dtReturn;
        }

        /// <summary>
        /// 分组筛选数据
        /// </summary>
        /// <param name="dtTitle">表名称</param>
        /// <param name="sourceDt">源数据表</param>
        /// <param name="selectExp">筛选语句</param>
        /// <returns></returns>
        private DataTable GroupDt(string dtTitle, DataTable sourceDt, string selectExp,string siteUrl,string resultList)
        {
            DataTable dtReturn = CreatCombinedDataTable(dtTitle);            
            DataRow[] drs = sourceDt.Select(selectExp);
            for (int i = 0; i < drs.Length; i++)
            {
                DataRow dr = dtReturn.NewRow();
                dr[0] = drs[i]["ID"];//ID 0
                dr[1] = drs[i]["Title"];//标题 1
                dr[2] = drs[i]["AType"];//类别ID 2
                dr[3] = drs[i]["AType"];//类别名称 3

                //当计划开始时间为空时，以创建时间为准 计划开始 4
                if (!Convert.IsDBNull(drs[i]["PlanDate"]))
                {
                    dr[4] = ((DateTime)drs[i]["PlanDate"]).Date;//计划开始
                }
                else
                {
                    dr[4]= ((DateTime)drs[i]["Created"]).Date;//创建时间
                }

                //当计划时长为空时，记为0 计划时长 5
                dr[5] = Convert.IsDBNull(drs[i]["PlanDuring"]) ? 0 : drs[i]["PlanDuring"];//计划时长

                //当实际开始时间为空时，以计划开始时间为准  实际开始 6
                if (!Convert.IsDBNull(drs[i]["ActualDate"]))
                {
                    dr[6] = ((DateTime)drs[i]["ActualDate"]).Date;//实际开始
                }
                else
                {
                    dr[6] = dr[4];//实际开始=计划开始 
                }

                //当实际时长为空时，记为0  实际时长 7
                dr[7] = Convert.IsDBNull(drs[i]["ActualDuring"]) ? 0 : drs[i]["ActualDuring"];//实际时长
                if (!Convert.IsDBNull(drs[i]["ID"]))
                {
                    string Id = drs[i]["ID"].ToString();
                    int activiId = int.Parse(Id);
                    int result = GetActivityResults(activiId,siteUrl,resultList);
                    dr[8] = result;//活动结果数 8
                }
                dr[9] = drs[i]["Author"];//用户ID 9
                dr[10] = drs[i]["Author"];//用户名 10
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        private int GetActivityResults(int activiId, string siteUrl, string resultList)
        {
            int results = 0;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        SPList spList = spWeb.Lists.TryGetList(resultList);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            qry.Query = "<Where><Eq><FieldRef Name='AssistantID' /><Value Type='Number'>" + activiId + "</Value></Eq></Where>";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                            {
                                results = listItems.Count;
                            }
                        }
                    }
                }
            });
            return results;
        }
     
        public DataTable MDataTable(DataTable dt1, DataTable dt2, String KeyColName)
        {
            //定义临时变量
            DataTable dtReturn = new DataTable();
            int i = 0;
            int j = 0;
            int k = 0;
            int colKey1 = 0;
            int colKey2 = 0;
            //设定表dtReturn的名字?
            dtReturn.TableName = dt1.TableName;
            //设定表dtReturn的列名
            for (i = 0; i < dt1.Columns.Count; i++)
            {
                if (dt1.Columns[i].ColumnName == KeyColName)
                {
                    colKey1 = i;
                }
                dtReturn.Columns.Add(dt1.Columns[i].ColumnName);
            }
            for (j = 0; j < dt2.Columns.Count; j++)
            {
                if (dt2.Columns[j].ColumnName == KeyColName)
                {
                    colKey2 = j;
                    continue;
                }
                dtReturn.Columns.Add(dt2.Columns[j].ColumnName);
            }
            //建立表的空间
            for (i = 0; i < dt1.Rows.Count; i++)
            {
                DataRow dr;
                dr = dtReturn.NewRow();
                dtReturn.Rows.Add(dr);
            }
            //将表dt1,dt2的数据写入dtReturn
            for (i = 0; i < dt1.Rows.Count; i++)
            {
                int m = -1;
                //表dt1的第i行数据拷贝到dtReturn中去
                for (j = 0; j < dt1.Columns.Count; j++)
                {
                    dtReturn.Rows[i][j] = dt1.Rows[i][j].ToString();
                }
                //查找的dt2中KeyColName的数据,与dt1相同的行
                for (k = 0; k < dt2.Rows.Count; k++)
                {
                    if (dt1.Rows[i][colKey1].ToString() == dt1.Rows[k][colKey2].ToString())
                    {
                        m = k;
                    }
                }
                //表dt2的第m行数据拷贝到dtReturn中去,且不要KeyColName(ID)列
                if (m != -1)
                {
                    for (k = 0; k < dt2.Columns.Count; k++)
                    {
                        if (k == colKey2)
                        {
                            continue;
                        }
                        dtReturn.Rows[i][j] = dt2.Rows[m][k].ToString();
                        j++;
                    }
                }
            }
            return dtReturn;
        }

        private void GetPersonalizedData(string planTitle, string planList, string siteUrl)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            SPList spList = spWeb.Lists.TryGetList(planList);
                            if (spList != null)
                            {
                                SPQuery qry = new SPQuery();
                                qry.Query = "";

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

        private DataTable GetDatas(string planList, string actionList, string actionTypeList, string resultList, string siteUrl)
        {
            DataTable dtReturn = new DataTable();
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            SPList pList = spWeb.Lists.TryGetList(planList);
                            SPList aList = spWeb.Lists.TryGetList(actionList);
                            SPList atLsit = spWeb.Lists.TryGetList(actionTypeList);
                            SPQuery qry = new SPQuery();
                            DateTime miniDate = DateTime.Now.AddDays(-90).Date;
                            qry.Query = @"<Where><Geq><FieldRef Name='Created' /><Value Type='DateTime'>"+ miniDate + "</Value></Geq></Where>";
                            //qry.ViewFields = @"<FieldRef Name='Title'/><FieldRef Name='PlanStart'/><FieldRef Name='PlanDuring'/><FieldRef Name='CustAction'/>";
                            //string[] fld1 = new string[] { "标题","计划开始","计划时长","活动操作"};
                            //string[] fld2 = new string[] { "标题","操作类别"};

                            //个人学习助手表
                            
                            DataTable dt1 = pList.GetItems(qry).GetDataTable();
                            //dt1.Columns.Remove("ID");
                            //dt1.Columns.Remove("Title");
                            //dt1.Columns.Remove("Created");
                            //dt1.Columns.Remove("Modified");
                            //dt1.AcceptChanges();

                            //操作表
                            //qry.ViewFields = @"<FieldRef Name='Title'/><FieldRef Name='Types'/>";

                            DataTable dt2 = aList.GetItems().GetDataTable();

                            //dt2.Columns.Remove("Created");
                            //dt2.Columns.Remove("Modified");
                            //dt2.AcceptChanges();
                            //DataTable dt0 = MixDT(dt1,"Action",dt2,"ActionType","AType");
                            //操作类别表
                            //qry.ViewFields = @"<FieldRef Name='Title' />";
                            DataTable dt3 = atLsit.GetItems().GetDataTable();
                            //dt3.Columns.Remove("Created");
                            //dt3.Columns.Remove("Modified");
                            //dt3.AcceptChanges();

                            //操作---操作类别表
                            //DataTable dt4=MixDT(dt2,"Types",dt3,"Title","AType");

                            //dt2.Columns.Remove("ID");
                            //dt2.Columns.Remove("Title");
                            //dt2.AcceptChanges();

                            //活动---操作类别表
                            dtReturn = MixDT(dt1, "CustAction", dt2, "Type", "AType");
                            //return dt5;
                        }
                    }
                }
                catch (Exception ex)
                {

                    //lbErr.Text = ex.ToString();
                    dtReturn = null;
                }
            });
            return dtReturn;
        }

        /// <summary>
        /// Chart初始化
        /// </summary>
        /// <param name="mychart">chart控件Id</param>
        /// <param name="chartTitle">chart标题</param>
        /// <param name="chartType">chart图像类型</param>
        private void InitChart(Chart mychart,string chartTitle, SeriesChartType chartType)
        {
            //图表标题
            mychart.Titles.Clear();
            mychart.Titles.Add(chartTitle);
            mychart.Titles[0].Text = chartTitle;
            mychart.Titles[0].ForeColor = Color.Red;
            //清除默认的series
            mychart.Series.Clear();
            for (int i = 0; i < SERIES.Length; i++)
            {
                Series series = new Series(SERIES[i]);
                series.ChartType = chartType;
                series.MarkerBorderWidth = 2;
                series.MarkerSize = 4;
                series.Name = SERIES[i];
                series.MarkerStyle = MarkerStyle.Circle;
                series.ToolTip = SERIES[i] + " #VAL \r\n #AXISLABEL";
                series["PieLabelStyle"] = "Inside";//将文字移到外侧
                //series["PieLineColor"] = "Black";//绘制黑色的连线。
                series.XValueType = ChartValueType.String;
                series.Label = "#VAL";
                if (series.ChartType==SeriesChartType.Pie)
                {
                    series.LabelToolTip = "#PERCENT{P}";
                }
                series.ShadowColor = Color.Black;
                series.BorderColor = Color.Brown;
                series.LegendText = SERIES[i];
                mychart.Series.Add(series);
            }
           
        }

        /// <summary>
        /// 图像
        /// </summary>
        private string[] SERIES =
        {
            "计划时长","实际时长"
        };

        /// <summary>
        /// 将统计数据绑定到chart控件
        /// </summary>
        /// <param name="dt">元数据</param>
        /// <param name="chart1">chart控件ID</param>
        /// <param name="chartTitle">chart标题</param>
        /// <param name="xField">绑定X坐标的元数据列</param>
        /// <param name="yFields">绑定Y坐标的元数据列,可以有多个，用数组表示 </param>
        /// <param name="sChartType">chart类型，如：pie（饼图），line（线型图）</param>
        private void ChartBindWeek(DataTable dt, Chart chart1, string chartTitle, string xField, string[] yFields, SeriesChartType sChartType)
        {
            InitChart(chart1, chartTitle, sChartType);//初始化Chart图像 

            //图表数据源
            chart1.Series[0].Points.Clear();
            chart1.Titles.Clear();
            chart1.DataSource = dt;

            //图表数据样式
            //for (int i = 0; i < yFields.Length; i++)
            //{
            //    chart1.Series[i].XValueMember = xField;
            //    chart1.Series[i].YValueMembers = yFields[i];
            //}
            List<string> xlist = new List<string>();
            List<double> ylist1 = new List<double>();
            List<double> ylist2 = new List<double>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                xlist.Add(dt.Rows[i][0].ToString());
                ylist1.Add(double.Parse(dt.Rows[i][1].ToString()));
                ylist2.Add(double.Parse(dt.Rows[i][2].ToString()));
            }
            chart1.Series[0].Points.DataBindXY(xlist, ylist1);
            chart1.Series[1].Points.DataBindXY(xlist, ylist2);

            chart1.Series[0].Color = Color.Red;
            chart1.Series[1].Color = Color.Blue;
        }

        public DataTable MixDT(DataTable dt1, string key1, DataTable dt2, string key2, string newCol)
        {
            //DataTable dtReturn = new DataTable();
            //dtReturn.TableName = dt1.TableName;
            //dtReturn = dt1.Clone();

            dt1.Columns.Add(newCol, typeof(string));
            //int k1 = 0, k2 = 0, k3 = 0;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                DataRow dr = dt1.Rows[i];
                string keyValue = dr[key1].ToString();//找出主表（外键）关联字段的值
                int keyindex = keyValue.IndexOf(";");
                if (keyindex > 0)
                {
                    string keyId = keyValue.Substring(0, keyValue.IndexOf(";"));
                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {
                        if (keyId == dt2.Rows[j]["ID"].ToString())
                        {
                            dt1.Rows[i][newCol] = dt2.Rows[j][key2].ToString();
                            break;
                        }
                    }
                }
                else
                {
                    dt1.Rows[i][newCol] = "未知";
                }
            }
            return dt1;
        }

        #endregion



        protected void rbPeriodList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbPeriodList.SelectedValue=="0")
            {
                divTimeSpan.Visible = true;
            }
            else
            {
                divTimeSpan.Visible = false;
            }
            
        }

        protected void btnDrawRadar_Click(object sender, EventArgs e)
        {
            string planList = webObj.PlanList;
            string siteUrl = webObj.SiteUrl;
            string actionList = webObj.ActionList;
            string actionTypeList = webObj.ActionTypeList;
            string resultList = webObj.ResultList;
            try
            {
                DataTable dtCombined = (DataTable)Session["元数据表"];
                DataTable dtTypes = (DataTable)Session["操作类别表"];
                if (dtCombined != null)
                {
                    DataTable dtComputed = DTComputeByTypeInPeriods(dtCombined, dtTypes);
                    if (dtComputed == null)
                    {
                        lbErr.Text = "活动类别雷达图无数据，不显示";
                        chartRadar.Visible = false;
                    }
                    else
                    {
                        RadarBind(dtComputed, "活动类别雷达图");
                    }
                }
                else
                {
                    chartRadar.Visible = false;
                    lbErr.Text = "无数据";
                }
            }
            catch (Exception ex)
            {
                lbErr.Text = ex.ToString();
            }
        }
    }
}
