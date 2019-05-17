using Microsoft.SharePoint;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RoutinePlan.DataVisualization
{
    public partial class DataVisualizationUserControl : UserControl
    {
        public DataVisualization webObj;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    DataTable sourceDt = GetPersonalizedData();

                    //DataTable dt = GroupDt();
                }
                catch (Exception ex)
                {

                    lbErr.Text=ex.ToString();
                }
            }
        }

        /// <summary>
        /// 分组筛选数据
        /// </summary>
        /// <param name="dtTitle">表名称</param>
        /// <param name="sourceDt">源数据表</param>
        /// <param name="selectExp">筛选语句</param>
        /// <returns></returns>
        private DataTable GroupDt(string selectExp)
		{
			DataTable dtReturn = new DataTable();
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


            DataTable sourceDt = GetPersonalizedData();
			DataRow[] drs = sourceDt.Select(selectExp);

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



		private DataTable GetPersonalizedData()
		{
            DataTable dt = new DataTable();
			SPSecurity.RunWithElevatedPrivileges(delegate ()
		   {
			   try
			   {
                   string siteUrl = webObj.SiteUrl;
				   using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
				   {
					   using (SPWeb spWeb = spSite.OpenWeb())
					   {
                           string planList = webObj.PlanList;
						   SPList spList = spWeb.Lists.TryGetList(planList);
						   if (spList != null)
						   {
							   SPQuery qry = new SPQuery();
                               qry.Query = @"<Where><Eq><FieldRef Name='Author' /><Value Type='Integer'><UserID /></Value>
      </Eq></Where>";
                               dt = spList.GetItems(qry).GetDataTable();
                           }
					   }
				   }
			   }
			   catch (Exception ex)
			   {

				   lbErr.Text = ex.ToString();
			   }
		   });
            return dt;
		}


    }
}
