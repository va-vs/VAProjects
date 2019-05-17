using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.DataVisualization;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.Linq;
using System.Linq;

namespace RoutinePlan.PersonlizedPlan
{
	public partial class PersonlizedPlanUserControl : UserControl
	{
		public PersonlizedPlan webObj { get; set; }
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				string planList = webObj.PlanList;
				string siteUrl = webObj.SiteUrl;
				string actionList = webObj.ActionList;
				string actionTypeList = webObj.ActionTypeList;
				try
				{
					DataTable dt = GetDatas(planList, actionList, actionTypeList, siteUrl);
                    SPUser user =SPContext.Current.Web.CurrentUser;

					string gpExp ="AType='项目'";
                    if (user!=null)//当前网站用户已登录
                    {
                        gpExp += " and Author='"+user.Name+"'";
                    }
                    DataTable dtNew = GroupDt("源数据表", dt,gpExp);

					gv1.DataSource = dtNew;
					gv1.DataBind();
					DataTable dtDates = GetDates(7, "PlanStart");
					DataTable dtResult = DTCompute(dtNew, "PlanStart", "PlanDuring",dtDates);
					//string[] distinctCols = new string[] { "UserName" };
					//DataTable dtResult =ComputeDT(dtNew, distinctCols, "sum(PlanDuring)", "");
					gv2.DataSource = dtResult;
					gv2.DataBind();
					ChartBind(dtResult, Chart1, "近一周项目类每日计划活动时长变化趋势", "PlanDuring", "PlanStart", SeriesChartType.Line);
				}
				catch (Exception ex)
				{

					lbErr.Text=ex.ToString();
				}
				//BindRoutinePlan(rPlanList, siteUrl);
			}
		}

		private static DataTable GetDates(int days,string field)
		{

			using (DataTable dtResult = new DataTable())
			{
				dtResult.Columns.Add(field);
				DateTime dtNow = DateTime.Now.Date;
				for (int i = days; i >0; i--)
				{
					dtResult.Rows.Add(new object[] { dtNow.AddDays(-i)});
				}
				return dtResult;
			}
		}

		private static DataTable ComputeDT(DataTable dt,string[] distinctCols,string computeExp,string filter)
		{
			DataTable dtResult = dt.Clone();
			DataTable dtName = dt.DefaultView.ToTable(true, distinctCols);
			for (int i = 0; i < dtName.Rows.Count; i++)
			{
				var builder = new System.Text.StringBuilder();
				for (int j = 0; j < distinctCols.Length; j++)
				{
					builder.Append(distinctCols[j] + "='" + dtName.Rows[i][j]+"'");
					if (j!=distinctCols.Length-1)
					{
						builder.Append(" and ");
					}
				}
				string selectExp = builder.ToString();
				//"name='" + dtName.Rows[i][0] + "' and sex='" + dtName.Rows[i][1] + "'"
				DataRow[] rows = dt.Select(selectExp);
				//temp用来存储筛选出来的数据
				DataTable temp = dtResult.Clone();
				foreach (DataRow row in rows)
				{
					temp.Rows.Add(row.ItemArray);
				}

				DataRow dr = dtResult.NewRow();
				dr[0] = dtName.Rows[i][0].ToString();
				dr[1] = temp.Compute(computeExp, filter);
				dtResult.Rows.Add(dr);
			}


			//var query = from t in dt.AsEnumerable()
			//            group t by new { t1 = t.Field<string>("name"), t2 = t.Field<string>("sex") } into m
			//            select new
			//            {
			//                name = m.Key.t1,
			//                sex = m.Key.t2,
			//                score = m.Sum(n => n.Field<decimal>("score"))
			//            };
			//if (query.ToList().Count > 0)
			//{
			//    query.ToList().ForEach(q =>
			//    {

			//    });
			//}

			return dtResult;
		}

		/// <summary>
		/// 合计
		///</summary>
		/// <param name="dt">元数据表</param>
		/// <param name="groupField">分组列</param>
		/// <param name="computeField">计算列</param>
		/// <param name="dtCompute">分组元数据</param>
		/// <returns></returns>
		public static DataTable DTCompute(DataTable dt,string groupField,string computeField,DataTable dtCompute)
		{
			DataTable dtResult = new DataTable();
			dtResult.Columns.Add(groupField);
			dtResult.Columns.Add(computeField);
			if (dtCompute==null)//未设定筛选数据表表
			{
				dtCompute=dt.DefaultView.ToTable(true, groupField);
			}
			for (int i = 0; i < dtCompute.Rows.Count; i++)
			{
				string filter = groupField+" = '" + dtCompute.Rows[i][0] + "'";
				dtResult.Rows.Add(new object[] { dtCompute.Rows[i][0], dt.Compute("sum(" + computeField + ")", filter) });
			}
			return dtResult;
		}


        /// <summary>
        /// 获取指定字段不重复项，返回List
        /// </summary>
        /// <param name="dataTable">元数据表</param>
        /// <param name="distinctCol">指定字段名</param>
        /// <returns></returns>
        public static List<string> GetDistinctColValuesFromDataTable(DataTable dataTable,string distinctCol)
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

        /// <summary>
        /// 分组筛选数据
        /// </summary>
        /// <param name="dtTitle">表名称</param>
        /// <param name="sourceDt">源数据表</param>
        /// <param name="selectExp">筛选语句</param>
        /// <returns></returns>
        private DataTable GroupDt(string dtTitle,DataTable sourceDt,string selectExp)
		{
			DataTable dtReturn = new DataTable(dtTitle);
			dtReturn.Columns.Add("ID", typeof(string));//ID
			dtReturn.Columns.Add("Title", typeof(string));//标题
			dtReturn.Columns.Add("Types", typeof(string));//类别
			dtReturn.Columns.Add("PlanStart", typeof(DateTime));//计划开始
			dtReturn.Columns.Add("PlanDuring", typeof(float));//计划时长
			dtReturn.Columns.Add("Start", typeof(DateTime));//实际开始
			dtReturn.Columns.Add("During", typeof(float));//实际时长
			dtReturn.Columns.Add("Results", typeof(int));//结果数量
			//dtReturn.Columns.Add("UserID",typeof(string));//用户ID
			dtReturn.Columns.Add("UserName", typeof(string));//用户名

			DataRow[] drs = sourceDt.Select(selectExp);
			//lbErr.Text = drs.Length.ToString() + ";";
			for (int i = 0; i < drs.Length; i++)
			{
				DataRow dr = dtReturn.NewRow();
				dr[0] = drs[i]["ID"];//ID
				dr[1] = drs[i]["Title"];//标题
				dr[2] = drs[i]["AType"];//类别
				if (!Convert.IsDBNull(drs[i]["PlanDate"]))
				{
					dr[3] = ((DateTime)drs[i]["PlanDate"]).Date;//计划开始
				}

				dr[4] = Convert.IsDBNull(drs[i]["PlanDuring"])? 0 : drs[i]["PlanDuring"];//计划时长
				if (!Convert.IsDBNull(drs[i]["ActualDate"]))
				{
					dr[5] = ((DateTime)drs[i]["ActualDate"]).Date;//实际开始
				}
				else
				{
					dr[5] = dr[3];
				}
				dr[6] = Convert.IsDBNull(drs[i]["ActualDuring"])?dr[4] : drs[i]["ActualDuring"];//实际时长
				if (!Convert.IsDBNull(drs[i]["ID"]))
				{
					string Id = drs[i]["ID"].ToString();
					int activiId = int.Parse(Id);
					int result = GetActivityResults(activiId);
					dr[7] = result;//活动结果数
				}
				//dr[8] = drs[i]["Author"];//用户ID
				dr[8] = drs[i]["Author"];//用户名
				dtReturn.Rows.Add(dr);
			}
			//var builder = new System.Text.StringBuilder();
			//builder.Append(lbErr.Text);

			//for (int i = 0; i < dtReturn.Columns.Count; i++)
			//{
			//    builder.Append(dtReturn.Rows[0][i] + ";");
			//}
			//lbErr.Text = builder.ToString();
			//lbErr.Text = builder.ToString();

			return dtReturn;
		}

		private int GetActivityResults(int activiId)
		{
			int result = 0;
			SPSecurity.RunWithElevatedPrivileges(delegate ()
			{
				try
				{
					string siteUrl = webObj.SiteUrl;
					string resultList = webObj.ResultList;
					using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
					{
						using (SPWeb spWeb = spSite.OpenWeb())
						{
							SPList spList = spWeb.Lists.TryGetList(resultList);
							if (spList != null)
							{
								SPQuery qry = new SPQuery();
								qry.Query ="<Where><Eq><FieldRef Name='AssistantID' /><Value Type='Number'>"+activiId+"</Value></Eq></Where>";
								SPListItemCollection listItems = spList.GetItems(qry);
								if (listItems.Count>0)
								{
									result = listItems.Count;
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
			return result;
		}
		private void BindRoutinePlan(string rPlanList, string siteUrl)
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
									ddlRPlans.DataTextField = "Title";
									ddlRPlans.DataValueField = "ID";
									ddlRPlans.DataSource = spList.GetItems();
									ddlRPlans.DataBind();
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

		public static DataTable MDataTable(DataTable dt1, DataTable dt2, String KeyColName)
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

		private static DataTable GetDatas(string planList,string actionList,string actionTypeList, string siteUrl)
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
						   //SPQuery qry = new SPQuery();
						   //qry.Query = "";
						   //qry.ViewFields = @"<FieldRef Name='Title'/><FieldRef Name='PlanStart'/><FieldRef Name='PlanDuring'/><FieldRef Name='CustAction'/>";
						   //string[] fld1 = new string[] { "标题","计划开始","计划时长","活动操作"};
						   //string[] fld2 = new string[] { "标题","操作类别"};

						   //个人学习助手表
						   DataTable dt1 = pList.GetItems().GetDataTable();
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
						   dtReturn= MixDT(dt1,"CustAction",dt2,"Type","AType");
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
		/// 将统计数据绑定到chart控件
		/// </summary>
		/// <param name="dt">元数据</param>
		/// <param name="chart1">chart控件ID</param>
		/// <param name="chartTitle">chart标题</param>
		/// <param name="xField">绑定X坐标的元数据列</param>
		/// <param name="yField">绑定Y坐标的元数据列</param>
		/// <param name="sChartType">chart类型，如：pie（饼图），line（线型图）</param>
		private void ChartBind(DataTable dt, Chart chart1, string chartTitle, string xField, string yField,SeriesChartType sChartType)
		{
			chart1.Series[0].Points.Clear();
			chart1.Titles.Clear();

			List<string> listX = new List<string>();
			List<double> listY = new List<double>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				listX.Add(dt.Rows[i][xField].ToString());

				listY.Add(double.Parse(dt.Rows[i][yField].ToString()));
			}
			chart1.Titles.Add(chartTitle);
			chart1.Titles[0].Text = chartTitle;
			chart1.Titles[0].ForeColor = Color.Red;
			chart1.Series[0].Points.DataBindXY(listX, listY);
			//chart1.Series[0].Points.DataBindY(listY);
			chart1.Series[0].ChartType = sChartType;
			chart1.Series[0]["PieLabelStyle"] = "Inside";//将文字移到外侧
			chart1.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线。
			chart1.Series[0].XValueType = ChartValueType.String;
			chart1.Series[0].Label = "#VAL";
			chart1.Series[0].LabelToolTip = "#PERCENT{P}";
			//chart1.Series[0].Label = "#VALY";
			chart1.Series[0].LegendText = "#VALX";
		}

		public static DataTable MixDT(DataTable dt1,string key1,DataTable dt2,string key2,string newCol)
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
				if (keyindex>0)
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

		/// <summary>
		/// 组合两个DataTable
		/// </summary>
		/// <param name="dt1">第一个DataTable</param>
		/// <param name="dt2">第二个DataTable</param>
		/// <param name="KeyColName1">关联的列在表1中的列名</param>
		/// <param name="KeyColName2">关联的列在表2中的列名</param>
		/// <param name="SameColName">表1和表2中列名相同但不做关联的列</param>
		/// <returns></returns>
		public static DataTable MergeDataTable(DataTable dt1, DataTable dt2, String KeyColName1,string KeyColName2,string SameColName)
		{
			//定义临时变量
			DataTable dtReturn = new DataTable();
			int i = 0;
			int j = 0;
			int k = 0;
			int colKey1 = 0;
			int colKey2 = 0;
			int skey1 = 0;
			int skey2 = 0;
			//设定表dtReturn的名字?
			dtReturn.TableName = dt1.TableName;
			//设定表dtReturn的列名
			for (i = 0; i < dt1.Columns.Count; i++)
			{
				if (dt1.Columns[i].ColumnName == KeyColName1)
				{
					colKey1 = i;
				}
				if (dt1.Columns[i].ColumnName == SameColName)
				{
					skey1 = i;
				}
				dtReturn.Columns.Add(dt1.Columns[i].ColumnName);
			}
			for (j = 0; j < dt2.Columns.Count; j++)
			{
				if (dt2.Columns[j].ColumnName == KeyColName2)
				{
					colKey2 = j;
					//continue;
				}
				if (dt2.Columns[j].ColumnName == SameColName)
				{
					skey2 = j;
					//continue;
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
						if (k==skey2)
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

	}
}
