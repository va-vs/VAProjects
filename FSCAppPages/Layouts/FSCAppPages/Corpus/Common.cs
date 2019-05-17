using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{
    public class Common
    {
        #region 处理外键
        /// <summary>
        /// 获取语料的外键的文本值，多选项
        /// </summary>
        /// <param name="typeName">扩展表中的类型，对应于语料表的字段名去ID</param>
        /// <param name="dtCorpusExtends">扩展表的数据表</param>
        /// <param name="ids">多选的ID值，对应于字段的值</param>
        /// <returns></returns>
        public static string GetTitlesByIDs(DataTable dtCorpusExtends, string ids, string typeName, string splitStr)
        {
            string strTitles="";
            string[] idKeys = Regex.Split(ids, splitStr);
            DataRow[] drs;
            foreach (string id in idKeys)
            {
                if (id.Length > 0)
                {
                    drs = dtCorpusExtends.Select("Types='" + typeName + "' and ItemID=" + id);
                    if (drs.Length > 0)
                        strTitles +=drs[0]["Title"].ToString() + splitStr;
                }
            }
            if (strTitles.Length > 0)
                strTitles = strTitles.Substring(0, strTitles.Length - 1);
            return strTitles;
        }
        /// <summary>
        /// 获取一个控件的多个选项作为查询条件
        /// </summary>
        /// <param name="cbl">多选框</param>
        /// <param name="fieldName">查询条件中的字段名称</param>
        /// <param name="splitStr">分隔符</param>
        /// <returns></returns>
        public static string GetQueryString(CheckBoxList cbl, string fieldName, string splitStr)
        {
            string selval = "";
            var builder = new StringBuilder();
            builder.Append(selval);
            for (int i = 0; i < cbl.Items.Count; i++)
            {
                if (cbl.Items[i].Selected)
                {
                    if (builder.ToString() == "")
                        builder.Append(fieldName + " like '%" + splitStr + cbl.Items[i].Value + splitStr+"%'");
                    else
                        builder.Append(" or " + fieldName + " like '%" + splitStr + cbl.Items[i].Value + splitStr+"%'");
                }
            }
            selval = builder.ToString();
            return selval;
        }
        /// <summary>
        /// 获取CheckBoxList中选中了的值
        /// </summary>
        /// <param name="cbl">CheckBoxList</param>
        /// <param name="splitStr">分割符号,例如"1;2;4"中的分号</param>
        /// <returns></returns>
        public static string GetCBListChecked(CheckBoxList cbl, string splitStr)
        {
            string selval = "";
            var builder = new StringBuilder();
            if (cbl.SelectedIndex >= 0)
                builder.Append(splitStr);
            else
                builder.Append(selval);
            for (int i = 0; i < cbl.Items.Count; i++)
            {
                if (cbl.Items[i].Selected)
                {
                    builder.Append(cbl.Items[i].Value + splitStr);
                }
            }
            selval = builder.ToString();
            return selval;
        }
        /// <summary>
        /// 设置CheckBoxList中选中项
        /// </summary>
        /// <param name="cbl">CheckBoxList</param>
        /// <param name="selectedValue">选中了的值串,例如："1;2;4;"</param>
        /// <param name="splitStr">值串中使用的分割符,例如"1;2;4;"中的分号</param>
        public static void SetCBListChecked(CheckBoxList cbl, string selectedValue, string splitStr)
        {
            //selectedValue = splitStr + selectedValue;//例如："1;2;4;"->";1;2;4;"
            for (int i = 0; i < cbl.Items.Count; i++)
            {
                cbl.Items[i].Selected = false;//先让所有选项处于未选中状态

                string val = splitStr + cbl.Items[i].Value + splitStr;
                if (selectedValue.IndexOf(val) != -1)//存在如“;1;”这样的子串
                {
                    cbl.Items[i].Selected = true;
                    selectedValue = selectedValue.Replace(val, splitStr);//然后从原来的值串中用分隔符替换已经选中了的
                    if (selectedValue == splitStr)//selectedValue的最后一项也被选中，此时selectedValue仅剩下一个分隔符
                    {
                        return;//跳出循环，不再执行
                    }
                }
            }
        }
        #endregion
    }
}
