
using System;
using System.Collections;
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
        #region 句子中检索单词
        /// <summary>
        /// 
        /// </summary>
        /// <param name="findWord">要查询的单词</param>
        /// <param name="strContent">文本内容</param>
        /// <param name="position">单词的匹配位置；-1：左匹配，0：中间匹配，1：匹配</param>
        /// <param name="wordsCount">显示的其他单词的个数</param>
        /// <returns></returns>
        public static List<List<string>> FindWord(string findWord, string strContent, int position, int wordsCount)
        {
            string ignore = "[\r\n\t\"]";//需要替换的符号
            strContent = Regex.Replace(strContent, ignore, " ");
            strContent = Regex.Replace(strContent, "\\s{2,}", " ");
            //s{2,} 中的s表示空格，数字2表示两个或以上的空格
            List<List<string>> results = new List<List<string>>();
            List<string> words = ParseWords(strContent);
            int wordIndex = words.FindIndex(word => word == findWord);//要找的两个单词相距单词个数与参数个数相同
            int leftIndex;
            int rightIndex;
            int start;
            string resultWord;//前面或后面的单词
            while (wordIndex > -1)
            {
                resultWord = "";
                if (position >= 0)//右匹配，找左边的单词
                {
                    leftIndex = wordIndex - wordsCount;
                    if (leftIndex < 0) leftIndex = 0;
                    start = leftIndex;
                    while (start < wordIndex)
                    {
                        resultWord += " " + words[start];
                        start += 1;
                    }
                    resultWord = resultWord.Trim();
                }
                else
                    resultWord = "";
                words.Add(resultWord);

                words.Add(findWord);
                //处理中间
                if (position > 0)
                    resultWord = "";
                else
                {
                    rightIndex = wordIndex + wordsCount;
                    if (rightIndex >= words.Count) rightIndex = words.Count - 1;
                    start = wordIndex + 1;
                    while (start <= rightIndex)
                    {
                        resultWord += " " + words[start];
                        start += 1;
                    }
                    resultWord = resultWord.Trim();

                }
                words.Add(resultWord);
                //三部分的数组
                results.Add(words);
              
                wordIndex = words.FindIndex(wordIndex + wordsCount, word => word == findWord);
            }

            return results;
        }
        #endregion
        #region 处理句子与单词
        /// <summary>
        /// 获取文本中的句子
        /// </summary>
        /// <param name="strContent">英文文本</param>
        /// <returns>键值集合，键为序列号，值为单词集合 </returns>
        public static Dictionary<int, List<string>> ParseSentences(string strContent)
        {
            string ignore = "[\r\n\t\"]";//需要替换的符号
            strContent = Regex.Replace(strContent, ignore, " ");
            Regex rx = new Regex(@"(\S.+?[.!?])(?=\s+|$)");
            MatchCollection matchs = rx.Matches(strContent);
            Dictionary<int, List<string>> sentences = new Dictionary<int, List<string>>();
            string sentence;
            List<string> words = new List<string>();
            foreach (Match match in matchs)
            {
                sentence = match.Value;
                words = ParseWords(sentence);
                sentences.Add(match.Index, words);

            }
            return sentences;
        }
        /// <summary>
        /// 解析句子中的单词
        /// </summary>
        /// <param name="sentence">包含符号的句子</param>
        /// <returns></returns>
        public static List<string> ParseWords(string sentence)
        {
            Regex reg = new Regex(@"\b\w+\b");
            MatchCollection mc = reg.Matches(sentence);
            List<string> words = new List<string>();
            foreach (Match m in mc)
            {
                words.Add(m.Value);
            }
            return words;
        }

        /// <summary>
        /// 计算一组数字序列的标准差
        /// </summary>
        /// <param name="values">数字序列</param>
        /// <returns>标准差</returns>
        private static double CalculateStdDev(List<int> values)
        {
            double ret = 0;
            if (values.Count > 0)
            {
                //  计算平均数
                double avg = values.Average();
                //  计算各数值与平均数的差值的平方，然后求和
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //  除以数量，然后开方
                ret = Math.Sqrt(sum / values.Count());
            }
            return ret;
        }

        #endregion
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
            string strTitles = "";
            string[] idKeys = Regex.Split(ids, splitStr);
            DataRow[] drs;
            foreach (string id in idKeys)
            {
                if (id.Length > 0)
                {
                    drs = dtCorpusExtends.Select("Types='" + typeName + "' and ItemID=" + id);
                    if (drs.Length > 0)
                        strTitles += drs[0]["Title"].ToString() + splitStr;
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
                        builder.Append(fieldName + " like '%" + splitStr + cbl.Items[i].Value + splitStr + "%'");
                    else
                        builder.Append(" or " + fieldName + " like '%" + splitStr + cbl.Items[i].Value + splitStr + "%'");
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

        #region DataTable方法

        /// <summary>
        /// 行列转置
        /// </summary>
        /// <param name="dt">源数据表</param>
        /// <returns>转置后的数据表</returns>
        public static DataTable TranspositionDT(DataTable dt)
        {
            DataTable dtResult = new DataTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtResult.Columns.Add("Col" + i);
            }
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                DataRow drNew = dtResult.NewRow();
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    string value = String.Empty;
                    DataRow drOld = dt.Rows[r];
                    if (drOld[c] != null && !Convert.IsDBNull(drOld[c]) && drOld[c].ToString() != "null")
                    {
                        value = drOld[c].ToString();
                    }
                    drNew[r] = value;
                }
                dtResult.Rows.Add(drNew);
            }
            return dtResult;
        }

        #endregion DataTable方法
    }
}
