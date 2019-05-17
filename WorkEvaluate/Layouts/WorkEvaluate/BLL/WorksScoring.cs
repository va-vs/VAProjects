using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace WorkEvaluate.Layouts.WorkEvaluate.BLL
{
    /// <summary>
    /// 类内容:作品分配,评分,点评,阳历作品评价训练
    /// </summary>
    public class WorksScoring
    {
        /// <summary>
        /// 计算指定期次下作品的分值并更新到作品表
        /// </summary>
        /// <param name="periodID"></param>
        public static int ComputerAllScoresByPeriod(long periodID)
        {
            DataSet ds = DAL.Works.GetWorksByPeriodID(periodID);
            Single result;
            using (SqlTransaction trans = DAL.DataProvider.CurrentTransactionEx)
                try
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result = ComputerPerworksScore((long)dr["WorksID"]);
                        if (result > 0)
                        {
                            dr["Score"] = result;
                            dr["WorksState"] = 3;
                            DAL.Works.UpdateWorksSubmit(trans, dr);
                        }
                    }
                    trans.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return 0;
                }
        }
        /// <summary>
        /// 计算成绩(去掉0分)，去掉最高、最低两个成绩，其他求平均分
       /// </summary>
       public static Single ComputerPerworksScore(long worksID)
       {
           DataSet ds=DAL.Works.GetWorksExpertByWorksID(worksID );
           DataRow[] drs = ds.Tables[0].Select("Score>0");
           if (drs.Length == 0)
               return 0;
           int i, j;
           if (drs.Length  >6 )
           {
               i = 2;
               j = drs.Length - 3;

           }
           else if (drs.Length > 4)
           {
               i = 1;
               j = drs.Length - 2;
           }
           else
           {
               i = 0;
               j = drs.Length - 1;
           }
           Single scores = 0;
           DataRow dr;
           int k = i;
           while (i<=j)
           {
               dr = drs[i];
               //if (!dr.IsNull("Score"))
               scores += (Single)dr["Score"];
               i += 1;
           }
           scores = scores / (j - k + 1);
           return scores;
       }
        /// <summary>
       /// 对筛选的DataSet扩展两个自定义列
        /// </summary>
        /// <param name="periodId"></param>
        /// <returns></returns>
       public static DataSet PeriodWorksDataSetExtend(long periodId)
       {
           DataSet ds = DAL.Works.GetUserIdWorksIdByPeriodId(periodId); //查询出的源数据集

           ds.Tables[0].Columns.Add("allotnum", typeof(int));//为表创建一新列,存放作品分配次数
           ds.Tables[0].Columns.Add("hasallot", typeof(int));//创建一新列,存放用户已分配作品数
           ds.Tables[0].Columns.Add("StateID", typeof(int));//创建一新列,作品状态

           foreach (DataRow dr in ds.Tables[0].Rows)//将新列分别递归添加到对应的行中
           {
               dr["allotnum"] = 0;
               dr["hasallot"] = 0;
               dr["StateID"] = 1;
           }
          
           return ds;
       }
       public static DataSet FenpeiDs(long periodId,long userId)
       {

           DataSet ds = PeriodWorksDataSetExtend(periodId); //扩展后的数据集
           DataView dv=new DataView();
          
           DataTable dtuw = ds.Tables[0];
           int dtrcount = dtuw.Rows.Count;
           if (dtrcount < 2)
           {
               //Response.Write();
               return null;
           }
           else
           {
               dv.Table = ds.Tables[0];
               dv.Sort = "UserID";//按用户ID排序
               dv.RowFilter = "UserID<>" + userId.ToString() + "";//查询不是userId创建的的作品
               int sortNum =dtrcount>9?9:dtrcount-1;
               for (int i = 0; i < dtuw.Rows.Count; i++)
               {
                   DataRow dr =dv.ToTable().Rows[i];
                   if (dr["UserID"] != userId.ToString()) //对某个用户userId进行筛选并分配
                   {

                   }
               }

           }
         
           DataTable dt = new DataTable();
           ds.Tables[0].Columns.Add("allotnum", typeof(int));//直接为表创建一新列,存放作品分配次数
           ds.Tables[0].Columns.Add("StateID", typeof(int));//直接为表创建一新列,作品状态

           foreach (DataRow dr in ds.Tables[0].Rows)//将新列分别递归添加到对应的行中
           {

               dr["allotnum"] = 0;//GetPaymount()为一个自定义方法
               dr["StateID"] = 1;
           }
           //return dal.GetListNotPayment(strWhere);
           return ds;
       }
       public void SelectRowDataTable(long periodId)
       {
           DataTable dt = PeriodWorksDataSetExtend(periodId).Tables[0];
           DataRow[] drArr = dt.Select();//查询(Select内无条件，就是查询所有的数据)
           for (int i = 0; i < drArr.Length; i++)
           {
               long expertId = Convert.ToInt64(drArr[0].Table.Rows[i]["UserID"].ToString());
               DataRow[] drnotIn = dt.Select("UserID='" + expertId + "'");//查询不是expertId的作品
               long[] aa = new long[drnotIn.Length];
               long[] bb = new long[drnotIn.Length-1];
               for (int j = 0; j < drnotIn.Length; j++)
               {
                   aa[j] = Convert.ToInt64(drArr[0].Table.Rows[i]["UserID"].ToString());//转化为数组
               }
               DataTable dtnew=new DataTable();
           }
           //还可以这样操作：
           DataRow[] drArr1 = dt.Select("C1 LIKE 'abc%'");//模糊查询（如果的多条件筛选，可以加 and 或 or ）
           DataRow[] drArr2 = dt.Select("'abc' LIKE C1 + '%'", "C2 DESC");//另一种模糊查询的方法
           DataRow[] drArr3 = dt.Select("C1='abc'", "C2 DESC");//排序

           //问题又来了，如果要把DataRow赋值给新的DataTable，怎么赋值呢？你可能会想到： 
           DataTable dtNew1 = dt.Clone();
           for (int i = 0; i < drArr.Length; i++)
           {
               dtNew1.Rows.Add(drArr[i]);
           }

           //但这样程序就会出错，说该DataRow是属于其他DataTable的，那要怎么做呢？很简单，这样就可以解决了： 
           DataTable dtNew2 = dt.Clone();
           for (int i = 0; i < drArr.Length; i++)
           {
               dtNew2.ImportRow(drArr[i]);//ImportRow 是复制
           }
       }
        /// <summary>
       /// 排除原始数组第n个元素后的新数组
        /// </summary>
        /// <param name="oldArr">原始数组</param>
        /// <param name="n">原始数组第n个元素</param>
       /// <returns>排除原始数组第n个元素后的新数组</returns>
        public long[] ArrCut(long[] oldArr,long n)
        {
            long[] newArr = new long[oldArr.Length-1];
            for (long i = 0; i < n; i++)
            {
                newArr[i] = oldArr[i];
            }
            for (long i = n + 1; i < oldArr.Length; i++)
            {
                newArr[i] = oldArr[i];
            }
            return newArr;
        }
    }
}
