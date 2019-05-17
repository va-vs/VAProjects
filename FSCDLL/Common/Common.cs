using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSCDLL.Common
{
    /// <summary>
    /// 通用的方法
    /// </summary>
    public class Common
    {
        #region 数据的比较
        /// <summary>
        /// dataRow比较
        /// </summary>
        /// <param name="drA"></param>
        /// <param name="drB"></param>
        /// <param name="columnNames">需要比较的列名称</param>
        /// <returns></returns>
        public static bool DataRowCompare(DataRow drA, DataRow drB, string[] columnNames)
        {
            bool flag = true;
            foreach (string colName in columnNames)
            {
                if (drA.Table.Columns.Contains(colName) && drB.Table.Columns.Contains(colName))
                {

                    //值比较
                    if (!CompareObject(drA[colName], drB[colName]))
                    {
                        flag = false;
                        break;
                    }

                }
            }
            return flag;
        }

        /// <summary>
        /// 按照数组中列名顺序排序
        /// </summary>
        /// <param name="drA"></param>
        /// <param name="columnNames">按照数组中列名顺序排序</param>
        public static void ColumnSort(DataRow drA, string[] columnNames)
        {
            //drA 排序
            int i = 0;
            foreach (string columnName in columnNames)
            {
                if (drA.Table.Columns.Contains(columnName))
                {
                    drA.Table.Columns[columnName].SetOrdinal(i);
                    i++;
                }
            }
        }

        /// <summary>
        /// 将从表格中筛选后的多个行转换成表格
        /// </summary>
        /// <param name="rows">行集合</param>
        /// <returns>表格</returns>
        public static DataTable RowsToTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();  // 复制DataRow的表结构
            foreach (DataRow row in rows)
                tmp.Rows.Add(row.ItemArray);  // 将DataRow添加到DataTable中
            return tmp;
        }
        #endregion
        #region 引用对象比较
        /// <summary>
        /// 引用对象比较
        /// </summary>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <returns></returns>
        public static bool CompareObject(object objA, object objB)
        {
            bool flag = false;
            if (objA == null || objB == null)
            {
                flag = false;
            }
            else if (objA == DBNull.Value && objB != DBNull.Value)
            {
                flag = false;
            }
            else if (objA != DBNull.Value && objB == DBNull.Value)
            {
                flag = false;
            }
            else if (objA == DBNull.Value && objB == DBNull.Value)
            {
                //objA objB 对应的列类型已经比较过 类型已判断 值一致
                flag = true;
            }
            else if (objA.GetType() != objB.GetType())
            {
                flag = false;
            }
            else if (objA is int || objA is short || objA is long || objA is float || objA is double || objA is decimal)
            {
                //int 01与1
                if (objA is int)
                {
                    if ((int)objA == (int)objB)
                    {
                        flag = true;
                    }
                }
                else if (objA is short)
                {
                    if ((short)objA == (short)objB)
                    {
                        flag = true;
                    }
                }
                else if (objA is long)
                {
                    if ((long)objA == (long)objB)
                    {
                        flag = true;
                    }
                }
                else if (objA is float)
                {
                    if ((float)objA == (float)objB)
                    {
                        flag = true;
                    }
                }
                else if (objA is double)
                {
                    if ((double)objA == (double)objB)
                    {
                        flag = true;
                    }
                }
                else if (objA is decimal)
                {
                    if ((decimal)objA == (decimal)objB)
                    {
                        flag = true;
                    }
                }
            }
            else if (objA is TimeSpan )
            {
                if (((TimeSpan )objA).TotalMinutes==((TimeSpan )objB).TotalMinutes)
                {
                    flag = true;
                }
            }
            else
            {
                string strA = objA.ToString ();
                string strB =  objB.ToString ();
                if (strA == strB)
                {
                    flag = true;
                }
            }
            return flag;
        }

        #endregion
        #region 数据数据集
        /// <summary>
        /// 将数据集转变为带层次结构的数据集
        /// </summary>
        /// <param name="parentIDName">父级ID</param>
        /// <param name="dtTasks">任务列表</param>
        /// <param name="titleName">标题</param>
        /// <param name="addBlankLine">是否在首行加空行</param>
        /// <returns></returns>
        public static DataTable GetDataTableByLevel(DataTable dtAllTasks, string titleName, string parentIDName, bool addBlankLine )
        {
            DataTable dtTasks = dtAllTasks.Copy();
            dtTasks.Columns[titleName].ColumnName = "Title";//列统一命名
            dtTasks.Columns[parentIDName].ColumnName = "ParentID";
            dtTasks.Columns[0].ColumnName = "ID";//第一列为编号列

            dtTasks.AcceptChanges();
            DataRow[] drs = dtTasks.Select("ParentID is null or ParentID=0"); //获取根结节的数据
            DataRow dr;
            DataTable dt = dtTasks.Clone();
            foreach (DataRow tmpTr in drs)
            {
                WriteDataTable(ref dt, tmpTr, 0);
                GetSubTables(dtTasks, (long)tmpTr[0], 1, ref dt);
            }
            if (addBlankLine)
            {
                dr = dt.NewRow();//首行加空数据
                dr["ID"] = 0;
                dt.Rows.InsertAt(dr, 0);
            }
            return dt;
        }
        /// <summary>
        /// 分层次显示任务，子任务前面加空格
        /// </summary>
        /// <param name="dt">表格</param>
        /// <param name="level">级别</param>
        /// <param name="drParent">父级的行</param>
        private static void WriteDataTable(ref DataTable dt, DataRow  drParent, int level)
        {
            //DataRow dr = dt.NewRow();
            //dr["ID"] = drParent[0];
            string preStr = "";
            var builder = new StringBuilder();
            builder.Append(preStr);
            for (int i = 0; i < level; i++)
                builder.Append(("&nbsp;&nbsp;&nbsp;&nbsp;"));
            preStr = builder.ToString();
            string tmp = drParent["Title"].ToString();
            drParent["Title"] = System.Web.HttpUtility.HtmlDecode(preStr) + tmp;
            //dt.Rows.Add(dr);
            dt.ImportRow(drParent);
        }
        private static void GetSubTables(DataTable dtTasks, long parentID, int level, ref DataTable dt)
        {
            DataRow[] drs = dtTasks.Select("ParentID=" + parentID); //获取根
            foreach (DataRow dr in drs)
            {
                WriteDataTable(ref dt, dr, level);
                GetSubTables(dtTasks, (long)dr["ID"], level + 1, ref dt);
            }
        }

        /// <summary>
        /// 将两个拥有一个公共列的结构不同的DataTable合并成一个新的DataTable
        /// </summary>
        /// <param name="dt1">表dt1</param>
        /// <param name="dt2">表dt2</param>
        /// <param name="DTName">合并后新的表名</param>
        /// <param name="colName">公共的列名,此列在表中是唯一值列</param>
        /// <returns>合并后的新表dt3</returns>
        private static DataTable UniteDataTable(DataTable dt1, DataTable dt2,string colName, string DTName)
        {
            DataTable dt3 = dt1.Clone();
            int cc1 = dt1.Columns.Count;
            int cc2 = dt2.Columns.Count;

            //第一步：结构合并，列拼接
            for (int i = 0; i < cc2; i++)
            {
                if (colName!=dt2.Columns[i].ColumnName)
                    dt3.Columns.Add(dt2.Columns[i].ColumnName);
            }
            object[] obj = new object[dt3.Columns.Count];

            //第二步：合并数据，注意相同列的值的对应
            int rc1 = dt1.Rows.Count;
            int rc2 = dt2.Rows.Count;
            //首先直接读取表1所有行，直接写入表3
            for (int i = 0; i < rc1; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }


            //合并表2
            if (rc1 >= rc2)//表1数据行数多于表2
            {
                for (int i = 0; i < rc2; i++)
                {
                    for (int j = 0; j < cc2; j++)
                    {
                        if (colName != dt2.Columns[j].ColumnName)
                        {
                            dt3.Rows[i][j + cc1] = dt2.Rows[i][j].ToString();
                        }
                    }
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < rc2 - rc1; i++)
                {
                    dr3 = dt3.NewRow();
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i <rc2; i++)
                {
                    for (int j = 0; j < cc2; j++)
                    {
                        dt3.Rows[i][j +cc1] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            dt3.TableName = DTName; //设置DT的名字
            return dt3;
        }
        #endregion
        #region 验证
        public static bool IsTelephone(string phoneValue)
        {
            string timeExpr = @"^(((\(\d{3}\)|\d{3}-)?\d{8})|((\(\d{4,5}\)|\d{4,5}-)?\d{7,8})|(\d{11}))$";
            Regex rex = new Regex(timeExpr);
            if (rex.IsMatch(phoneValue))
            {
                return true;
            }
            else
                return false;
        }
        public static bool IsEmail(string email)
        {
            string emailExpr = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            if (Regex.IsMatch(email, emailExpr))
                return true;
            else
                return false;
        }
        //只能输入中文
        public static bool IsChinese(string name)
        {
            if (name.Length > 0)
            {
                char[] chars = name.ToCharArray();
                var ret = true;
                for (var i = 0; i < chars.Length; i++)
                    ret = ret && (chars[i] >= 10000);
                return ret;
            }
            else
            {
                return false;
            }
        }
        //测试工号或学号
        public static bool IsMatching(string accString)
        {
            bool ismatch = true;
            ismatch = Common.isNumberic(accString);
            if (ismatch)
            {
                if (accString.Length >= 5 && accString.Length <= 8)
                    ismatch = true;
                else
                    ismatch = false;
            }

            return ismatch;
        }
        /// <summary>
        /// 判断输入的字符串是否只包含数字和英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        public static bool isNumberic(string message)
        {
            //if (message != "" && Regex.IsMatch(message, @"^\d{5}$"))
            Regex rex = new Regex(@"^\d+$");

            if (rex.IsMatch(message))
            {
                return true;
            }
            else
                return false;
        }
        #endregion
    }
}
