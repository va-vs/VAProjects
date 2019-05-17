using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Linq;

namespace VAStateIndicator.StateIndicator
{
    public partial class StateIndicatorUserControl : UserControl
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SPContext.Current.Web.CurrentUser == null)
                return;
            DataTable dtResult =null;
            try
            {
                dtResult = GetMyIndex();
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.ToString();
                return;
            }
            Table tbl = new Table();
            TableRow tRow;
            TableCell tCell;
            tbl.BorderWidth = 0;
            tbl.CssClass = "stable";
            tRow = new TableRow();

            tCell = new TableCell();
            //tCell.Width = 100;
            tCell.Text = "指标";
            tCell.Font.Bold = true;
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            //tCell.Width = 100;
            tCell.Text = "最新";
            tCell.Font.Bold = true;
            tCell.HorizontalAlign = HorizontalAlign.Center;
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            //tCell.Width = 100;
            tCell.Text = "最小";
            tCell.Font.Bold = true;
            tCell.HorizontalAlign = HorizontalAlign.Center;
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            //tCell.Width = 100;
            tCell.Text = "最大";
            tCell.Font.Bold = true;
            tCell.HorizontalAlign = HorizontalAlign.Center;
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            //tCell.Width = 120;
            tCell.Text = "统计";
            tCell.Font.Bold = true;
            tCell.HorizontalAlign = HorizontalAlign.Center;
            tRow.Cells.Add(tCell);

            //tCell = new TableCell();
            ////tCell.Width = 100;
            //tCell.Text = "次数";
            //tCell.Font.Bold = true;
            //tCell.HorizontalAlign = HorizontalAlign.Center;
            //tRow.Cells.Add(tCell);

            tCell = new TableCell();
            //tCell.Width = 100;
            tCell.Text = "趋势";
            tCell.Font.Bold = true;
            tCell.HorizontalAlign = HorizontalAlign.Center;
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            //tCell.Width = 100;
            tCell.Text = "排行榜";
            tCell.Font.Bold = true;
            tCell.HorizontalAlign = HorizontalAlign.Center;
            tRow.Cells.Add(tCell);

            tbl.Rows.Add(tRow);
            foreach (DataRow dr in dtResult.Rows )
            {
                tRow = new TableRow();

                tCell = new TableCell();
                tCell.Text = dr["Title"].ToString() ;
                tCell.Font.Bold = true;
                tRow.Cells.Add(tCell);

                tCell = new TableCell();
                tCell.Text =dr.IsNull ("IndexValueLatest")?"0":dr["IndexValueLatest"].ToString ();
                tRow.Cells.Add(tCell);
                tCell.HorizontalAlign = HorizontalAlign.Center;

                tCell = new TableCell();
                tCell.Text = dr.IsNull("IndexValueMin") ? "0" : dr["IndexValueMin"].ToString();
                tRow.Cells.Add(tCell);
                tCell.HorizontalAlign = HorizontalAlign.Center;

                tCell = new TableCell();
                tCell.Text = dr.IsNull("IndexValueMax") ? "0" : dr["IndexValueMax"].ToString();
                tRow.Cells.Add(tCell);
                tCell.HorizontalAlign = HorizontalAlign.Center;

                tCell = new TableCell();
                tCell.Text = dr.IsNull("IndexValueTotal") ? "0" : dr["IndexValueTotal"].ToString();
                tCell.HorizontalAlign = HorizontalAlign.Center;
                tRow.Cells.Add(tCell);

                //tCell = new TableCell();
                //tCell.Text = dr.IsNull("IndexValueCount") ? "" : dr["IndexValueCount"].ToString();
                //tCell.HorizontalAlign = HorizontalAlign.Center;
                //tRow.Cells.Add(tCell);

                tCell = new TableCell();
                tCell.Text = dr.IsNull("IndexValueTrend") ? "-" : dr["IndexValueTrend"].ToString();
                tCell.HorizontalAlign = HorizontalAlign.Center;
                tRow.Cells.Add(tCell);

                tCell = new TableCell();
                tCell.Text = dr.IsNull("IndexValueRank") ? "" : dr["IndexValueRank"].ToString();
                tCell.HorizontalAlign = HorizontalAlign.Center;
                tRow.Cells.Add(tCell);

                tbl.Rows.Add(tRow);

            }
            divContent.Controls.Add(tbl);

        }
        #endregion
        #region 属性
        public StateIndicator  webObj { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 获取我的指标（从个人状态指标列表中读取）
        /// </summary>
        /// <returns></returns>
        private DataTable GetMyIndex()
        {
            DataTable dtIndex = GetListData(webObj.IndicatorListName);//指标列表

            DataColumn dataColumn = new DataColumn("IndexValueLatest", typeof(string));

            dtIndex.Columns.Add(dataColumn);

            dataColumn = new DataColumn("IndexValueMin", typeof(string));

            dtIndex.Columns.Add(dataColumn);

            dataColumn = new DataColumn("IndexValueMax", typeof(string));

            dtIndex.Columns.Add(dataColumn);

            dataColumn = new DataColumn("IndexValueTotal", typeof(string));

            dtIndex.Columns.Add(dataColumn);


            dataColumn = new DataColumn("IndexValueTrend", typeof(string));

            dtIndex.Columns.Add(dataColumn);

            dataColumn = new DataColumn("IndexValueRank", typeof(string));

            dtIndex.Columns.Add(dataColumn);


            long userID = GetAuthorID();
            //获取所有的数据
            DataTable dtAll = GetListData (webObj.ListName );// GetActivityByUser(webObj.ListName, userID);//获当前用户的所有状态指标
            //获取当前用户的数据
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            string username = currentUser.Name;
            DataRow[] drs = dtAll.Select("Author= '" + username + "'");
             DataTable dtActivity = dtAll.Clone();
            DataSet ds = new DataSet();
            ds.Tables.Add(dtActivity);
             
            ds.Merge(drs);
            //当前用户不存在或者用户没有录入指标数据
            if (userID == 0 || dtActivity.Rows.Count ==0) return dtIndex;

            string[] users = GetNamesFromDataTable(dtAll.Copy () , "Author");//获取个人助手中所有的用户

            DataRow drCurrentUserRank;//= GetRankByIndicatorandUser(dtAll, indexTitle, users);
            DataTable resultTable = resultDatatable;

            foreach (DataRow dr in dtIndex.Rows)//找下面的指标规则
            {
                string indexTitle = dr["Title"].ToString();

                
                //1、最新指标值
                //DataTable dt = DtSelectTop(2, dtActivity, "Index='" + indexTitle + "'", "Created desc");//指标为indexTitle，且按创建时间倒序的前两条数据
                DataRow[] rows = dtActivity.Select("Index='" + indexTitle + "'", "Created desc");
                if (rows.Length  > 0)
                    dr["IndexValueLatest"] = Convert.IsDBNull(rows[0]["value"]) ? "0" : rows[0]["value"].ToString();

                else
                    dr["IndexValueLatest"] = "0";

                //2、最小值
                string minValue = dtActivity.Compute("min(value)", "Index='" + indexTitle + "'").ToString();
                dr["IndexValueMin"] = DecimalStr(minValue, 2);

                //3、最大值
                string maxValue = dtActivity.Compute("max(value)", "Index='" + indexTitle + "'").ToString();
                dr["IndexValueMax"] = DecimalStr(maxValue, 2);

                //4、统计指标值
                //if (indexTitle == "3H值")
                //{

                //    string avgValue = dtActivity.Compute("avg(value)", "Index='" + indexTitle + "'").ToString();//平均值
                //    dr["IndexValueTotal"] = "(Avg) " + DecimalStr(avgValue, 2);
                //}
                //else
                //{
                //    string sumValue = dtActivity.Compute("sum(value)", "Index='" + indexTitle + "'").ToString();//和值
                //    dr["IndexValueTotal"] = "(Sum) " + DecimalStr(sumValue, 2);
                //}
                drCurrentUserRank = GetRankByIndicatorandUser(dtAll, indexTitle, users,resultTable.Clone ());
                dr["IndexValueTotal"] = drCurrentUserRank==null?0: drCurrentUserRank["得分"];
                //5、记录数
                //string countValue = dtActivity.Compute("count(ID)", "Index='" + dr["Title"].ToString() + "'").ToString();//计数
                //dr["IndexValueCount"] =DecimalStr(countValue, 0);

                //6、日变化趋势
                if (rows.Length>= 2)
                {
                    string nowstr = Convert.IsDBNull(rows[0]["value"]) ? "0" : rows[0]["value"].ToString();//这一次
                    float nowvalue = float.Parse(DecimalStr(nowstr, 2));

                    string lastStr = Convert.IsDBNull(rows[1]["value"]) ? "0" : rows[1]["value"].ToString();
                    float lastvalue = float.Parse(DecimalStr(lastStr, 2));//上一次
                    if (nowvalue > lastvalue)
                    {
                        dr["IndexValueTrend"] = "↑";
                    }
                    else if (nowvalue < lastvalue)
                    {
                        dr["IndexValueTrend"] = "↓";
                    }
                    else
                    {
                        dr["IndexValueTrend"] = "-";
                    }
                }
                else
                {
                    dr["IndexValueTrend"] = "-";
                }

                //7、排行
                dr["IndexValueRank"] = drCurrentUserRank==null?"": drCurrentUserRank["排行"] ;// GetRankByIndicatorandUser(dtAll , indexTitle,users);
            }
            dtIndex.AcceptChanges();
            return dtIndex;
        }
        /// <summary>
        /// 通过规则进行计算（暂时手工录入，规则待定）
        /// </summary>
        /// <param name="listName">获取数据的列表名称</param>
        /// <returns></returns>
        public static DataTable GetListData(string listName)
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
                            qry.Query ="<OrderBy><FieldRef Name='Created' Ascending='FALSE'/></OrderBy>";
                            SPListItemCollection items = spList.GetItems(qry);
                            retDataTable = items.GetDataTable();
                        }
                    }
                }
            });
            return retDataTable;
        }

        //获取用户的活动
        public static  DataTable GetActivityByUser(string listName,long userID)
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
                            qry.Query = @"<Where><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Lookup'>" + userID + "</Value></Eq></Where>";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                retDataTable = listItems.GetDataTable();

                        }
                    }
                }
            });
            return retDataTable;
        }
        //获取登陆用户的账号
        public static string GetAccount(SPUser currentUser)
        {
            if (currentUser == null)
                return "";
            string loginName = currentUser.LoginName;
            //if (currentUser.Name != "系统帐户")
                loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        //获取导入数据的创建者信息，匿名用户返回0
        public static  int GetAuthorID()
        {

            SPUser lUser = SPContext.Current.Web.CurrentUser;
            int id = 0;
            if (lUser !=null)
                id=lUser.ID;
            return id;
        }

        private DataTable _resultDatatable = null;
        /// <summary>
        /// 初始化汇总结果表
        /// </summary>
        /// <returns></returns>
        public DataTable resultDatatable
        {
            get
            {
                if (_resultDatatable == null)
                {
                    DataTable dt = new DataTable();//指标列表
                    dt.Columns.Add(new DataColumn("用户", typeof(string)));
                    dt.Columns.Add(new DataColumn("指标", typeof(string)));
                    dt.Columns.Add(new DataColumn("得分", typeof(float)));
                    dt.Columns.Add(new DataColumn("排行", typeof(int)));
                    _resultDatatable = dt.Clone();
                }
                return _resultDatatable.Clone ();
            }
        }
        /// <summary>
        /// 获取当前用户的某一状态指标的排行
        /// </summary>
        /// <param name="dtAll">包括所有用户和全部指标的全部数据</param>
        /// <param name="Indicator"></param>
        /// <param name="users">所有用户</param>
        /// <returns></returns>
        public DataRow GetRankByIndicatorandUser(DataTable dtAll, string Indicator, string[] users, DataTable tempDT)
        {
            //string rank = "";
            //string[] users = GetNamesFromDataTable(listTable, "Author");
            DataRow[] drs;
            //DataTable tempDT = resultDatatable;
            for (int i = 0; i < users.Length; i++)//每个用户
            {
                DataRow dr = tempDT.NewRow();
                dr["用户"] = users[i];
                dr["指标"] = Indicator;
                //DataTable dtActivity = GetDataByAuthorName(webObj.ListName, users[i]);//获取指定用户ID的所有状态指标
                drs = dtAll.Select("Author='" + users[i] + "'");
                DataTable dtActivity = dtAll.Clone();
                DataSet ds = new DataSet();
                ds.Tables.Add(dtActivity);

                ds.Merge(drs);

                if (Indicator == "3H值")
                {
                    string avgValue = dtActivity.Compute("avg(value)", "Index='" + Indicator + "'").ToString();//平均值
                    dr["得分"] = DecimalStr(avgValue, 2);
                }
                else
                {
                    string sumValue = dtActivity.Compute("sum(value)", "Index='" + Indicator + "'").ToString();//平均值
                    dr["得分"] = DecimalStr(sumValue, 2);
                }
                tempDT.Rows.Add(dr);
            }
            DataTable newDt = DtSelectTop(0, tempDT, "", "得分 desc");
            for (int i = 0; i < newDt.Rows.Count; i++)
            {
                newDt.Rows[i]["排行"] =i+1;//排行改为数字
            }
            newDt.AcceptChanges();

            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            string username = currentUser.Name;
            drs = newDt.Select("用户= '" + username + "'");
            if (drs.Length > 0)
            {
                //rank = drs[0]["排行"].ToString();
                return drs[0];
            }
            return null;
        }
        /// <summary>
        /// 根据指标获取指标记录数据
        /// </summary>
        /// <param name="listName">数据来源列表名称</param>
        /// <param name="Indicator">指标名称</param>
        /// <returns></returns>
        public string GetRankByIndicatorandUser1(string listName,string Indicator)
        {
            string rank = "";
            string siteUrl = SPContext.Current.Site.Url;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(listName);
                        if (spList != null)
                        {
                            DataTable listTable = spList.GetItems().GetDataTable();
                            string[] users = GetNamesFromDataTable(listTable, "Author");
                            DataTable tempDT = resultDatatable;
                            for (int i = 0; i < users.Length; i++)
                            {
                                DataRow dr = tempDT.NewRow();
                                dr["用户"] = users[i];
                                dr["指标"] = Indicator;
                                DataTable dtActivity = GetDataByAuthorName(webObj.ListName, users[i]);//获取指定用户ID的所有状态指标
                                if (Indicator=="3H值")
                                {
                                    string avgValue=dtActivity.Compute("avg(value)", "Index='" + Indicator+ "'").ToString();//平均值
                                    dr["得分"] = DecimalStr(avgValue, 2);
                                }
                                else
                                {
                                    string sumValue = dtActivity.Compute("sum(value)", "Index='" + Indicator + "'").ToString();//平均值
                                    dr["得分"] =DecimalStr(sumValue, 2);
                                }
                                tempDT.Rows.Add(dr);
                            }
                            DataTable newDt = resultDatatable;
                            newDt = DtSelectTop(0,tempDT,"", "得分 desc");
                            for (int i = 0; i < newDt.Rows.Count; i++)
                            {
                                newDt.Rows[i]["排行"] = (i + 1).ToString();
                            }
                            newDt.AcceptChanges();

                            SPUser currentUser = SPContext.Current.Web.CurrentUser;
                            string username = currentUser.Name;
                            DataRow[] drs = newDt.Select("用户= '" + username + "'");
                            if (drs.Length>0)
                            {
                                rank = drs[0]["排行"].ToString();
                            }
                        }
                    }
                }
            });
            return rank;
        }

        /// <summary>
        /// 获取表格的某个字段筛选的唯一值
        /// </summary>
        /// <param name="dataTable">源表</param>
        /// <param name="field">过滤的字段</param>
        /// <returns></returns>
        public static string[] GetNamesFromDataTable(DataTable dataTable,string field)
        {
            DataView dv = dataTable.DefaultView;
            dataTable = dv.ToTable(true, field);
            string[] names = new string[dataTable.Rows.Count];
            
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = dataTable.Rows[i][0].ToString();
            }
            return names;
        }


        public static string[] GetAllUsers(SPList splist,string field)
        {
            //先确定你要获取哪栏（列）的不重复值，这里把voteboxlist换成你的列表实例，“得票人”换成你要处理的列名
            SPField distinctfield = splist.Fields[field];
            //准备一个二维对象数组做输出参数来接收列的结果（即你最后要的不重复值）
            object[,] distinctresult;
            //调用getdistinctfieldvalues后distinctresult就是不重复的值，而返回resultcount 就是有多少个值
            uint resultcount = splist.GetDistinctFieldValues(distinctfield, out distinctresult);
            string[] names = new string[resultcount];
            var results = from t in splist.Items.Cast<SPListItem>()
                          select new { t.Title };
            var disresults = Enumerable.Distinct(results);
            //for (int i = 0; i < resultcount; i++)
            //{
            //    names[i] = SPEncode.HtmlEncode(resultcount.GetValue(0, i).ToString());
            //}
            return names;
        }
        /// <summary>
        /// 根据创建者获取数据集
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="AuthorName"></param>
        /// <returns></returns>
        public static DataTable GetDataByAuthorName(string listName, string AuthorName)
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
                            SPQuery qry = new SPQuery();
                            qry.Query = @" <Where><Eq><FieldRef Name='Author' /><Value Type='User'>"+ AuthorName + "</Value></Eq></Where>";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                retDataTable = listItems.GetDataTable();

                        }
                    }
                }
            });
            return retDataTable;
        }

        #region 获取DataTable按指定字段排序后的前几条数据
        /// <summary>
        /// 获取DataTable前几条数据
        /// </summary>
        /// <param name="TopCount">前N条数据</param>
        /// <param name="oDT">源DataTable</param>
        /// <param name="sortStr">源表排序字段与方式，比如"ID asc"</param>
        /// <param name="selectStr">源表筛选条件</param>
        /// <returns></returns>
        public static DataTable DtSelectTop(int TopCount, DataTable oDT,string selectStr, string sortStr)
        {
            DataTable nDT = oDT.Clone();
            DataRow[] rows = oDT.Select(selectStr, sortStr);
            if (oDT.Rows.Count < TopCount || TopCount == 0)//筛选个数多余表的行数或者筛选条数为0均表示整个表全选
                TopCount = oDT.Rows.Count;
            for (int i = 0; i < TopCount; i++)
            {
                nDT.ImportRow((DataRow)rows[i]);
            }
            return nDT;
        }
        #endregion

        #region 数字字符串保留两位小数
        /// <summary>
        /// 将数字字符串四舍五入保留n位小数
        /// </summary>
        /// <param name="numStr">数字字符串</param>
        /// <param name="n">保留的小数位数</param>
        /// <returns></returns>
        public static string DecimalStr(string numStr,int n)
        {
            if (string.IsNullOrEmpty(numStr)) return "0";

            string outStr = numStr;
            if (outStr.Length - outStr.IndexOf('.')-1>n)
            {
                outStr=Math.Round(Convert.ToDecimal(numStr),n).ToString();
            }

            return outStr;
        }
        #endregion
        #endregion

    }
}
