using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Configuration;
using Microsoft.SharePoint.Utilities;

namespace ComputeData
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string planList =  ConfigurationManager.AppSettings["PlanList"];
            string actionList = ConfigurationManager.AppSettings["ActionList"];
            string actionTypeList = ConfigurationManager.AppSettings["ActionTypeList"];
            string siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
            string resultList = ConfigurationManager.AppSettings["ResultList"];
            string returnList = ConfigurationManager.AppSettings["ReturnList"];
            //DataTable dt = GetDatas(planList, actionList, actionTypeList, siteUrl);
            //获取元数据
            DataTable dtCombined = CombinTypeAndResultsToRecords("元数据", planList, siteUrl, actionList, resultList);
            Console.WriteLine("源数据列表“" + planList + "”共有 " + dtCombined.Rows.Count + " 条数据");
            int rowsCount= ComputeDataToList(dtCombined, siteUrl, returnList);
            Console.WriteLine("经计算后,共写入结果列表“"+ returnList+"” "+rowsCount+"条数据");
            Console.ReadLine();
        }
        private static DataTable CombinTypeAndResultsToRecords(string dtTitle, string pList, string siteUrl, string actionList, string resultList)
        {
            DataTable dtCombined = CreatCombinedDataTable(dtTitle);
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb())
                {
                    SPList spList = spWeb.Lists.TryGetList(pList);
                    if (spList != null)
                    {
                        SPQuery qry = new SPQuery();

                        qry.Query = @"<Where><IsNotNull><FieldRef Name='CustAction' /></IsNotNull></Where>";

                        SPListItemCollection items = spList.GetItems(qry);
                        if (items.Count > 0)
                        {
                            for (int i = 0; i < items.Count; i++)
                            {
                                DataRow dr = dtCombined.NewRow();
                                dr[0] = items[i]["ID"];//ID 0
                                dr[1] = items[i]["Title"];//标题 1
                                if (items[i]["CustAction"] != null)
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
            return dtCombined;
        }

        private static string GetTypeinActionListByID(int actionId, string siteUrl, string actionList)
        {
            string typeValue = "";
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
            return typeValue;
        }

        private static void DTCompute()
        {

        }

        private static DataTable CreatCombinedDataTable(string dtTitle)
        {
            DataTable dtReturn = new DataTable(dtTitle);//共11列
            dtReturn.Columns.Add("ID", typeof(string));//ID 0
            dtReturn.Columns.Add("对象", typeof(string));//标题 1
            dtReturn.Columns.Add("类别ID", typeof(string));//类别Id 2
            dtReturn.Columns.Add("类别名称", typeof(string));//类别名称 3 
            dtReturn.Columns.Add("计划开始", typeof(DateTime));//计划开始 4
            dtReturn.Columns.Add("计划时长", typeof(float));//计划时长 5
            dtReturn.Columns.Add("实际开始", typeof(DateTime));//实际开始 6
            dtReturn.Columns.Add("实际时长", typeof(float));//实际时长 7
            dtReturn.Columns.Add("结果数", typeof(int));//结果数量 8
            dtReturn.Columns.Add("用户ID", typeof(string));//用户ID 9
            dtReturn.Columns.Add("用户名", typeof(string));//用户名 10
            return dtReturn;
        }


        /// <summary>
        /// 通过读取个人学习助手、操作、操作类别列表，形成用户每天每个类别的个人计划时长、实际时长以及结果量的统计数据
        /// </summary>
        private static int ComputeDataToList(DataTable dtSource,string siteUrl,string returnList)
        {
            int rowsCount = 0;
            try
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        spWeb.AllowUnsafeUpdates = true;
                        SPList spList = spWeb.Lists.TryGetList(returnList);
                        
                        string[] timesFileds = new string[2] { "计划开始", "实际开始" };
                        string[] userFileds = new string[2] { "用户ID", "用户名" };
                        string[] typeFields = new string[2] { "类别ID", "类别名称" };
                        DateTime[] times = SelectMinAndMaxDates(dtSource, timesFileds);
                        DateTime minTime = times[0];
                        DateTime maxTime = times[1];
                        int days = DateDiff(minTime, maxTime);
                        DataTable UserDistinct = SelectDistinct(dtSource, userFileds);
                        DataTable TypeDistinct = SelectDistinct(dtSource, typeFields);
                        for (int i = 0; i < UserDistinct.Rows.Count; i++)
                        {
                            for (int j = 0; j < TypeDistinct.Rows.Count; j++)
                            {
                                for (int k = 0; k < days; k++)
                                {
                                    rowsCount++;
                                    string title = "";
                                    SPListItem newItem = spList.AddItem();
                                    
                                    //用户
                                    string userId= UserDistinct.Rows[i][0].ToString();
                                    SPUser user = spWeb.Users.GetByID(int.Parse(userId));
                                    if (user!=null)
                                    {
                                        newItem["用户"] = spWeb.EnsureUser(user.LoginName);
                                        title += user.Name;
                                    }
                                    

                                    //日期
                                    DateTime dateA = minTime.AddDays(k);
                                    newItem["日期"] = dateA;
                                    title += dateA;

                                    //类别
                                    string typeId=TypeDistinct.Rows[i][0].ToString();
                                    var lookupField = spList.Fields["类别"] as SPFieldLookup;
                                    var lookupList = spWeb.Lists[new Guid(lookupField.LookupList)];
                                    var lookupitem = lookupList.GetItemById(int.Parse(typeId));
                                    var lookupValue = new SPFieldLookupValue(lookupitem.ID, lookupitem.ID.ToString());
                                    newItem["类别"] = lookupValue;
                                    title += lookupitem.Title;

                                    newItem["标题"] = title;
                                    //计划时长
                                    string filter = "用户ID='"+ userId + "' and 类别ID='" + typeId + "' and 计划开始 = '"+ dateA + "'";
                                    newItem["计划时长"] = dtSource.Compute("sum(计划时长)",filter);

                                    //实际时长
                                    filter = "用户ID='" + userId + "' and 类别ID='" + typeId + "' and 实际开始 = '" + dateA + "'";                                    
                                    newItem["实际时长"] = dtSource.Compute("sum(计划时长)", filter);

                                    //结果数
                                    newItem["结果数"] = dtSource.Compute("sum(结果数)", filter);
                                    newItem.Update();

                                }
                            }
                        }
                        spWeb.AllowUnsafeUpdates = false;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return rowsCount;
        }

        /// <summary>
        /// 计算两个日期相差天数
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        private static int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());

            TimeSpan sp = end.Subtract(start);

            return sp.Days;
        }

        /// <summary>
        /// 按照fieldName从sourceTable中选择出不重复的行，
        /// 相当于select distinct fieldName1,fieldName2,,fieldNamen from sourceTable
        /// </summary>
        /// <param name="sourceTable">源DataTable</param>
        /// <param name="fieldNames">列名数组</param>
        /// <returns>一个新的不含重复行的DataTable，列只包括fieldNames中指明的列</returns>
        public static DataTable SelectDistinct(DataTable sourceTable, string[] fieldNames)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(fieldNames[0], sourceTable.Columns[fieldNames[0]].DataType);
            dt = sourceTable.DefaultView.ToTable(true, fieldNames);//得到目标
            return dt;
        }

        /// <summary>
        /// 获取指定日期类型的列中的日期范围
        /// </summary>
        /// <param name="sourceTable">元数据表</param>
        /// <param name="fieldNames">所有参与计算的日期时间列名</param>
        /// <returns>返回一个计算所得最小时间和最大时间的数组</returns>
        public static DateTime[] SelectMinAndMaxDates(DataTable sourceTable, string[] fieldNames)
        {
            DateTime[] minTimes = new DateTime[fieldNames.Length];
            DateTime[] maxTimes = new DateTime[fieldNames.Length];
            
            for (int i = 0; i < fieldNames.Length; i++)
            {
                minTimes[i] =(DateTime)sourceTable.Compute("min(" + fieldNames[i] + ")", "1=1");
                maxTimes[i] = (DateTime)sourceTable.Compute("max(" + fieldNames[i] + ")", "1=1");
            }            
            Array.Sort(minTimes);
            Array.Sort(maxTimes);
            DateTime[] times = new DateTime[2] { minTimes[0], maxTimes[maxTimes.Length - 1] };
            return times;
        }
        private DataTable CreatDT(string dtName)
        {
            DataTable newDT = new DataTable(dtName);
            newDT.Columns.Add("用户名",typeof(object));
            newDT.Columns.Add("活动类别", typeof(string));
            newDT.Columns.Add("日期", typeof(DateTime));
            newDT.Columns.Add("计划时长", typeof(string));
            newDT.Columns.Add("实际时长", typeof(string));
            newDT.Columns.Add("活动结果数", typeof(int));
            //newDT.Columns.Add("", typeof(string));
            return newDT;
        }

        private static DataTable GetDatas(string planList, string actionList, string actionTypeList, string siteUrl)
        {
            DataTable dtReturn = new DataTable();

            try
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        SPList pList = spWeb.Lists.TryGetList(planList);
                        SPList aList = spWeb.Lists.TryGetList(actionList);
                        SPList atLsit = spWeb.Lists.TryGetList(actionTypeList);

                        //个人学习助手表
                        DataTable dt1 = pList.GetItems().GetDataTable();

                        //操作表
                        DataTable dt2 = aList.GetItems().GetDataTable();

                        //操作类别表
                        DataTable dt3 = atLsit.GetItems().GetDataTable();

                        //操作---操作类别表
                        //DataTable dt4=MixDT(dt2,"Types",dt3,"Title","AType");

                        //活动---操作类别表
                        dtReturn = MixDT(dt1, "CustAction", dt2, "Type", "AType");
                        //return dt5;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                dtReturn = null;
            }
            return dtReturn;
        }

        
        private static bool isInThisType(string actionName,string typeName,DataTable dtAction)
        {
            bool isIn = false;
            DataRow[] drs=dtAction.Select(string.Format("Title={0}",typeName));
            for (int i = 0; i < drs.Length; i++)
            {
                if (actionName==drs[i][1].ToString())
                {
                    isIn = true;
                    break;
                }
            }
            
             return isIn;
        }

        /// <summary>
        /// 获取指定活动Id的结果数量
        /// </summary>
        /// <param name="activiId"></param>
        /// <returns></returns>
        private static int GetActivityResults(int activiId,string siteUrl, string resultList)
        {
            int result = 0;
            try
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
                                result = listItems.Count;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Console.Write( ex.ToString());
            }
            return result;
        }

        public static DataTable MixDT(DataTable dt1, string key1, DataTable dt2, string key2, string newCol)
        {
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
    }
}
