using Microsoft.SharePoint;
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace PersonalRemind.remind
{
    public partial class remindUserControl : UserControl
    {
        public remind webObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            string cUrl = Page.Request.Url.ToString();
            if (!cUrl.Contains("_layouts/15"))
            {
                shownotified.Visible = true;
                GetRemindContent();
            }
            else
            {
                shownotified.Visible = false;
            }
        }
        public void GetRemindContent()
        {
            string[] indicators = webObj.ChkItem.Split(';');
            StringBuilder upContent = new StringBuilder();
            upContent.AppendLine("<table class='myupdatetable'>");
            long userId = GetAuthorID();
            if (userId == 0)//未登录
            {
                shownotified.Visible = false;
            }
            else
            {
                DataTable dtSource = GetItemsByUserAndIndicator(webObj.ListName, 0, "");

                GetUpdatetoHtml(dtSource, userId);
                GetRankstoHtml(dtSource);
            }
        }

        private void GetUpdatetoHtml(DataTable dtSource, long userId)
        {
            string[] indicators = webObj.ChkItem.Split(';');
            StringBuilder upContent = new StringBuilder();
            upContent.AppendLine("<table class='myupdatetable'>");
            upContent.AppendLine("<thead><tr><th>指标项</th><th>最后状态值</th><th>最后更新日期</th><th>无状态天数</th></tr></thead><tbody>");
            for (int i = 0; i < indicators.Length; i++)
            {
                if (dtSource.Select("Author='" + GetAuthorName() + "' And Index='" + indicators[i] + "'").Length>0)
                {
                    string[] lastUpData = GetLastUpData(dtSource.Select("Author='" + GetAuthorName() + "' And Index='" + indicators[i] + "'"));

                    if (int.Parse(lastUpData[2]) > 0)
                    {
                        upContent.AppendLine("<tr><th>" + indicators[i] + "</th><td>" + lastUpData[0] + "</td><td>" + lastUpData[1] + "</td><td>" + lastUpData[2] + "</td></tr>");
                    }
                }
                else
                {

                }
            }
            upContent.AppendLine("</tbody></table>");
            updateDiv.InnerHtml = upContent.ToString();
        }

        private void GetRankstoHtml(DataTable dtSource)
        {
            DataTable dtUsers = dtSource.DefaultView.ToTable(true, "Author");
            DataTable dtIndicators = dtSource.DefaultView.ToTable(true, "Index");
            dtIndicators.DefaultView.Sort = "Index Asc";
            string computField = "value";
            DataTable dtRanksandScore = GetDataComputByGroupField(dtIndicators, computField, dtSource, dtUsers);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div id='cs_product'><ul>");
            int k = dtIndicators.Rows.Count;
            for (int i = 0; i <k; i++)
            {

                sb.AppendLine("<li>");
                sb.AppendLine(getranks(dtIndicators.Rows[i][0].ToString(), dtRanksandScore));
                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul></div><div id='cs_product_num'><ul>");
            for (int i = 0; i < k; i++)
            {
                sb.AppendLine("<li>" + dtIndicators.Rows[i][0].ToString()+"</li>");
            }
            sb.AppendLine("</ul></div>");
            levelDiv.InnerHtml = sb.ToString();
        }


        private string getranks(string indicator, DataTable dtRanksandScore)
        {
            StringBuilder htmlstr = new StringBuilder();
            DataTable dtmy = dtRanksandScore.Select("指标 = '" + indicator + "'").CopyToDataTable();
            htmlstr.AppendLine("<div style='width:100%;text-align:center;height:20px;font-size:14px;font-weight:bold;'>" + indicator + "排行榜</div><hr/><table class='myupdatetable'>");
            htmlstr.AppendLine("<thead><tr><th>排行</th><th>用户</th><th>指标</th><th>汇总</th></tr></thead><tbody>");
            int rowsCount = dtmy.Rows.Count;
            DataRow[] drs = dtmy.Select("用户 = '" + GetAuthorName() + "'");
            if (drs.Length>0)
            {
                int rank = int.Parse(drs[0]["排行"].ToString());
                if (rowsCount <= 5)
                {
                    for (int i = 0; i < rowsCount; i++)
                    {
                        if (i == rank - 1)
                        {
                            htmlstr.AppendLine("<tr style='font-weight:bold;color:red'><th>" + dtmy.Rows[i][3] + "</th><td><b title='您的排行'>" + dtmy.Rows[i][0] + " *</b></td><td>" + dtmy.Rows[i][1] + "</td><td>" + StringtoRound(dtmy.Rows[i][2].ToString()) + "</td></tr>");
                        }
                        else
                        {
                            htmlstr.AppendLine("<tr><th>" + dtmy.Rows[i][3] + "</th><td>" + dtmy.Rows[i][0] + "</td><td>" + dtmy.Rows[i][1] + "</td><td>" + StringtoRound(dtmy.Rows[i][2].ToString()) + "</td></tr>");
                        }
                    }
                }
                else
                {
                    if (rank <= 3)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == rank - 1)
                            {
                                htmlstr.AppendLine("<tr style='font-weight:bold;color:red'><th>" + dtmy.Rows[i][3] + "</th><td><b title='您的排行'>" + dtmy.Rows[i][0] + " *</b></td><td>" + dtmy.Rows[i][1] + "</td><td>" + StringtoRound(dtmy.Rows[i][2].ToString()) + "</td></tr>");
                            }
                            else
                            {
                                htmlstr.AppendLine("<tr><th>" + dtmy.Rows[i][3] + "</th><td>" + dtmy.Rows[i][0] + "</td><td>" + dtmy.Rows[i][1] + "</td><td>" + StringtoRound(dtmy.Rows[i][2].ToString()) + "</td></tr>");
                            }
                        }
                        htmlstr.AppendLine("<tr><th>……</th><td>……</td><td>……</td><td>……</td></tr>");
                        htmlstr.AppendLine("<th>" + dtmy.Rows[rowsCount - 1][3] + "(末尾)</th><td>" + dtmy.Rows[rowsCount - 1][0] + "</td><td>" + dtmy.Rows[rowsCount - 1][1] + "</td><td>" + StringtoRound(dtmy.Rows[rowsCount - 1][2].ToString()) + "</td></tr>");
                    }
                    else if (rank > 3 && rank < rowsCount)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            htmlstr.AppendLine("<tr><th>" + dtmy.Rows[i][3] + "</th><td>" + dtmy.Rows[i][0] + "</td><td>" + dtmy.Rows[i][1] + "</td><td>" + StringtoRound(dtmy.Rows[i][2].ToString()) + "</td></tr>");
                        }
                        htmlstr.AppendLine("<tr><th>……</th><td>……</td><td>……</td><td>……</td></tr>");
                        htmlstr.AppendLine("<tr style='font-weight:bold;color:red'><th>" + dtmy.Rows[rank - 1][3] + "</th><td><b title='您的排行'>" + dtmy.Rows[rank - 1][0] + " *</b></td><td>" + dtmy.Rows[rank - 1][1] + "</td><td>" + StringtoRound(dtmy.Rows[rank - 1][2].ToString()) + "</td></tr>");
                        htmlstr.AppendLine("<tr><th>……</th><td>……</td><td>……</td><td>……</td></tr>");
                        htmlstr.AppendLine("<th>" + dtmy.Rows[rowsCount - 1][3] + "(末尾)</th><td>" + dtmy.Rows[rowsCount - 1][0] + "</td><td>" + dtmy.Rows[rowsCount - 1][1] + "</td><td>" + StringtoRound(dtmy.Rows[rowsCount - 1][2].ToString()) + "</td></tr>");
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            htmlstr.AppendLine("<tr><th>" + dtmy.Rows[i][3] + "</th><td>" + dtmy.Rows[i][0] + "</td><td>" + dtmy.Rows[i][1] + "</td><td>" + StringtoRound(dtmy.Rows[i][2].ToString()) + "</td></tr>");
                        }
                        htmlstr.AppendLine("<tr><th>……</th><td>……</td><td>……</td><td>……</td></tr>");
                        htmlstr.AppendLine("<tr style='font-weight:bold;color:red'><th>" + dtmy.Rows[rowsCount - 1][3] + "</th><td><b title='您的排行'>" + dtmy.Rows[rowsCount - 1][0] + " *</b></td><td>" + dtmy.Rows[rowsCount - 1][1] + "</td><td>" + StringtoRound(dtmy.Rows[rowsCount - 1][2].ToString()) + "</td></tr>");
                    }
                }

            }
            else
            {

                if (rowsCount <= 5)
                {
                    for (int i = 0; i < rowsCount; i++)
                    {
                        htmlstr.AppendLine("<tr><th>" + dtmy.Rows[i][3] + "</th><td>" + dtmy.Rows[i][0] + "</td><td>" + dtmy.Rows[i][1] + "</td><td>" + StringtoRound(dtmy.Rows[i][2].ToString()) + "</td></tr>");
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        htmlstr.AppendLine("<tr><th>" + dtmy.Rows[i][3] + "</th><td>" + dtmy.Rows[i][0] + "</td><td>" + dtmy.Rows[i][1] + "</td><td>" + StringtoRound(dtmy.Rows[i][2].ToString()) + "</td></tr>");
                    }
                    htmlstr.AppendLine("<tr><th>……</th><td>……</td><td>……</td><td>……</td></tr>");
                    htmlstr.AppendLine("<tr><th>" + dtmy.Rows[rowsCount - 1][3] + "</th><td>" + dtmy.Rows[rowsCount - 1][0] + "</td><td>" + dtmy.Rows[rowsCount - 1][1] + "</td><td>" + StringtoRound(dtmy.Rows[rowsCount - 1][2].ToString()) + "</td></tr>");
                }
            }
            htmlstr.AppendLine("</tbody></table>");
            return htmlstr.ToString();
        }

        public string StringtoRound(string sourceStr)
        {
            if (sourceStr.Contains('.'))
            {
                return string.Format("{0:N2}", Convert.ToDecimal(sourceStr).ToString());
            }
            else
            {
                return sourceStr;
            }
        }

        /// <summary>
        /// 查询指定用户在指定指标项上的上一次有状态的日期
        /// </summary>
        /// <param name="drSource">源数据表</param>
        public string[] GetLastUpData(DataRow[] drSource)
        {
            string[] lastUpdata = new string[3];
            foreach (DataRow dr in drSource)
            {
                string indicatorValue = Convert.IsDBNull(dr["value"]) ? "0" :dr["value"].ToString(); //item["指标值"]!= null?item["指标值"].ToString():"0";
                if (indicatorValue!="0")
                {
                    lastUpdata[0]=indicatorValue;
                    DateTime indicatorDate= (DateTime)dr["Created"];
                    if (!Convert.IsDBNull(dr["JudgeDate"]))
                    {
                        indicatorDate=(DateTime)dr["JudgeDate"];
                    }
                    //DateTime indicatorDate = Convert.IsDBNull(dr["JudgeDate"])?(DateTime)dr["JudgeDate"]:(DateTime)dr["Created"];
                    //item["日期"] != null ? (DateTime)item["日期"]:(DateTime)item["创建时间"];
                    lastUpdata[1]=indicatorDate.ToShortDateString();
                    int spanDays = DateDiff(indicatorDate, DateTime.Now);
                    lastUpdata[2] = spanDays.ToString();
                    break;
                }
            }
            return lastUpdata;
        }

        /// <summary>
        /// 获取某一列的所有值
        /// </summary>
        /// <typeparam name="T">列数据类型</typeparam>
        /// <param name="dtSource">数据表</param>
        /// <param name="filedName">列名</param>
        /// <returns></returns>
        public static List<T> GetColumnValues<T>(DataTable dtSource, string filedName)
        {
            return (from r in dtSource.AsEnumerable() select r.Field<T>(filedName)).ToList<T>();
        }

        /// <summary>
        /// 根据分组项表遍历每项的计算分组字段的汇总值
        /// </summary>
        /// <param name="gpField">汇总分组字段</param>
        /// <param name="computField">汇总字段</param>
        /// <param name="dtSource">源数据表</param>
        /// <param name="dtUsers">分组项表</param>
        /// <param name="dtIndicators">todo: describe dtIndicators parameter on GetDataComputByGroupField</param>
        /// <returns></returns>
        private DataTable GetDataComputByGroupField(DataTable dtIndicators,string computField,DataTable dtSource,DataTable dtUsers)
        {
            DataTable dtResult =GetDTResult();

            for (int i = 0; i < dtIndicators.Rows.Count; i++)
			{
                DataTable dtTemp = GetDTResult();
                dtTemp.Clear();
                for (int j = 0; j < dtUsers.Rows.Count; j++)
                {
                    DataTable temp = dtSource.Select("Author='" + dtUsers.Rows[j][0] + "'").CopyToDataTable();
                    //temp用来存储筛选出来的数据

                    DataRow dr = dtTemp.NewRow();
                    dr[0] = dtUsers.Rows[j][0];
                    dr[1] = dtIndicators.Rows[i][0];
                    object computeValue = 0;
                    if (dtIndicators.Rows[i][0].ToString() == "3H值"||dtIndicators.Rows[i][0].ToString() == "健康")
                    {
                        computeValue = temp.Compute("avg(" + computField + ")", "Index='" + dtIndicators.Rows[i][0] + "'");
                        computeValue = Convert.IsDBNull(computeValue)?0:double.Parse(string.Format("{0:0.00}",computeValue));
                    }
                    else
                    {
                        computeValue = temp.Compute("sum(" + computField + ")", "Index='" + dtIndicators.Rows[i][0] + "'");
                        computeValue = Convert.IsDBNull(computeValue)?0:double.Parse(string.Format("{0:0.00}",computeValue));
                    }
                    dr[2] = computeValue;
                    dtTemp.Rows.Add(dr);
                }
                dtTemp.DefaultView.Sort = "汇总 Desc";
                int rank = 0;
                //DataRow[] drs = dtTemp.Select("指标= '" + dtIndicators.Rows[i][0] + "'");
                foreach (DataRow dr in dtTemp.Rows)
                {
                    rank++;
                    dr[3] = rank;
                    dtResult.Rows.Add(dr.ItemArray);
                }
            }
            //dtResult.DefaultView.Sort = "用户 Asc";
            return dtResult;
        }
        /// <summary>
        /// 初始化汇总结果表
        /// </summary>
        /// <returns></returns>
        public DataTable GetDTResult()
        {
            DataTable dt = new DataTable();//指标列表

            using (DataColumn dataColumn = new DataColumn("用户", typeof(string)))
            {
                dt.Columns.Add(dataColumn);
            }
            using (DataColumn dataColumn = new DataColumn("指标", typeof(string)))
            {
                dt.Columns.Add(dataColumn);
            }
            using (DataColumn dataColumn = new DataColumn("汇总", typeof(double)))
            {
                dt.Columns.Add(dataColumn);
            }
            using (DataColumn dataColumn = new DataColumn("排行", typeof(string)))
            {
                dt.Columns.Add(dataColumn);
            }
            return dt;
        }

        /// <summary>
        /// 获取列表内所有数据项
        /// </summary>
        /// <param name="listName">获取数据的列表名称</param>
        /// <returns></returns>
        public SPListItemCollection GetListData(string listName)
        {
            string webUrl = SPContext.Current.Web.Url;
            if (!string.IsNullOrEmpty(webObj.WebUrl))
            {
                webUrl = webObj.WebUrl;
            }

            SPListItemCollection items = null;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite spSite = new SPSite(webUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            SPList spList = spWeb.Lists.TryGetList(listName);
                            if (spList != null)
                            {
                                SPQuery qry = new SPQuery();
                                qry.Query = "<OrderBy><FieldRef Name='Created' Ascending='FALSE'/></OrderBy>";
                                items = spList.GetItems(qry);
                            }
                        }
                    }
                });
            }
            catch (Exception)
            {

                throw;
            }
            return items;
        }


        /// <summary>
        /// 获取指定用户的某个指标的所有状态值，按日期由近及远排序
        /// </summary>
        /// <param name="listName">数据列表名称</param>
        /// <param name="userID">用户ID</param>
        /// <param name="indicator">指标</param>
        /// <param name="orderbyField">todo: describe orderbyField parameter on GetItemsByUserAndIndicator</param>
        /// <returns></returns>
        public DataTable GetItemsByUserAndIndicator(string listName, long userID,string indicator)
        {
            string webUrl = SPContext.Current.Web.Url;
            if (!string.IsNullOrEmpty(webObj.WebUrl))
            {
                webUrl = webObj.WebUrl;
            }
            SPListItemCollection items = null;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite spSite = new SPSite(webUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            SPList spList = spWeb.Lists.TryGetList(listName);
                            if (spList != null)
                            {
                                SPQuery qry = new SPQuery();
                                if (userID != 0 && indicator != "")
                                {
                                    qry.Query = @"<Where><And><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Lookup'>" + userID + "</Value></Eq><Eq><FieldRef Name = 'Index'/><Value Type = 'Lookup'>" + indicator + "</Value></Eq></And></Where><OrderBy><FieldRef Name='JudgeDate' Ascending='FALSE' /></OrderBy>";
                                }
                                else
                                {
                                    qry.Query = @"<OrderBy><FieldRef Name='JudgeDate' Ascending='FALSE' /></OrderBy>";
                                }
                                items = spList.GetItems(qry);
                            }
                        }
                    }
                });
            }
            catch (Exception)
            {

                throw;
            }
            
            return items.GetDataTable();
        }


        private DataTable GetTableByUserAndIndicator(SPListItemCollection items)
        {
            DataTable dt = items.GetDataTable();
            return dt;
        }

        /// <summary>
        /// 获取当前登录账户的账户名
        /// </summary>
        /// <returns></returns>
        public static string GetAccount()
        {
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            string loginName = currentUser.LoginName;
            //if (currentUser.Name != "系统帐户")
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }


        /// <summary>
        /// 获取当前用户的ID
        /// </summary>
        /// <returns></returns>
        public static int GetAuthorID()
        {
            SPUser lUser = SPContext.Current.Web.CurrentUser;
            int id =lUser!=null? lUser.ID:0;
            return id;
        }


        public static string GetAuthorName()
        {
            SPUser lUser = SPContext.Current.Web.CurrentUser;
            return lUser.Name;
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

    }
}
