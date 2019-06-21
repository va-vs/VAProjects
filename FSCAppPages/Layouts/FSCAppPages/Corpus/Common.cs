
using FSCDLL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{
    public class Common
    {
        #region 处理词组
        /// <summary>
        /// 对查询的词组汇总计算
        /// 返回表的字段（match、totalTimes、phraseTimes）
        /// </summary>
        /// <param name="dtSearchResult">查询的结果</param>
        /// <returns></returns>
        public static DataTable CaculatePhrase(DataTable dtSearchResult)
        {
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("match", typeof(string));
            dtResults.Columns.Add("totalTimes", typeof(int));
            dtResults.Columns.Add("phraseTimes", typeof(int));
            DataTable dtMatchs = dtSearchResult.DefaultView.ToTable(true, "match");
            DataRow drNew;
            DataTable  dtTmp=null;

            string match;
            DataRow[] drs;
            foreach (DataRow drTmp in dtMatchs.Rows)
            {
                drNew = dtResults.Rows.Add();
                match = drTmp["match"].ToString();
                drNew["match"] = match;
                drNew["totalTimes"] = dtSearchResult.Compute("count(match)", string.Format("match='{0}'", match));
                drs = dtSearchResult.Select(string.Format("match='{0}'", match));
                dtTmp =drs.CopyToDataTable();
                 drNew["phraseTimes"] = dtTmp.DefaultView.ToTable(true, "CorpusID").Rows.Count;
                dtResults.DefaultView.Sort = "totalTimes Desc";
            }
            dtResults.DefaultView.Sort = "totalTimes Desc";
            return dtResults;
        }
        /// <summary>
        /// 返回包含五列的数据表（CorpusID,Title,left,match,right）
        /// </summary>
        /// <param name="findWord">要查询的单词</param>
        /// <param name="dtCorpus">包含查找单词的数据表</param>
        /// <param name="findLeftWordsCount">找词组，要匹配单词的左侧单词个数</param>
        /// <param name="findRightWordsCount">找词组，要匹配单词的右侧单词个数</param>
        /// <param name="leftWordsCount">要返回的词组左边单词个数</param>
        /// <param name="rightWordsCount">要返回的词组右边单词个数</param>
        /// <returns>DataTable</returns>
        public static DataTable GetPhraseFromCorpus(string findWord, DataTable dtCorpus, int findLeftWordsCount, int findRightWordsCount, int leftWordsCount, int rightWordsCount)
        {
            string ignore = "[\r\n\t\"]";//需要替换的符号
            string strContent;
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("CorpusID", typeof(long));
            dtResults.Columns.Add("Title", typeof(string));

            dtResults.Columns.Add("left", typeof(string));
            dtResults.Columns.Add("match", typeof(string));
            dtResults.Columns.Add("right", typeof(string));
            dtResults.Columns.Add("OriginalText", typeof(string));
            DataRow drNew;
            foreach (DataRow dr in dtCorpus.Rows)
            {
                strContent = dr["OriginalText"].ToString();
                strContent = Regex.Replace(strContent, ignore, " ");
                strContent = Regex.Replace(strContent, "\\s{2,}", " ");
                //s{2,} 中的s表示空格，数字2表示两个或以上的空格
                List<string> words = ParseWords(strContent);//先解析单词

                string phrase = GetPhrase(words, findWord, findLeftWordsCount, findRightWordsCount);//要构造的短语
                if (phrase.Length == 0) continue;
                //在段落中查询短语
                Regex rx = new Regex(@"(" + phrase + ")");// (@"(e\S*$)");//(a|e|s)

                MatchCollection matchs = rx.Matches(strContent);
                Dictionary<int, List<string>> findPhrase = new Dictionary<int, List<string>>();
                string leftContent;
                string rightContent;

                List<string> leftWords;
                List<string> rightWords;
                string leftStr;
                string rightStr;
                int iStart;
                foreach (Match match in matchs)
                {
                    int i = match.Index;//词组的索引
                    leftStr = "";
                    rightStr = "";
                    if (leftWordsCount > 0 && i > 0)
                    {
                        leftContent = strContent.Substring(0, i);
                        leftWords = ParseWords(leftContent);
                        iStart = leftWords.Count - leftWordsCount;
                        if (iStart < 0) iStart = 0;
                        while (iStart <= leftWords.Count - 1)
                        {
                            leftStr = leftStr + leftWords[iStart] + " ";
                            iStart += 1;
                        }
                        leftStr = leftStr.Trim();
                    }
                    else
                        leftStr = "";
                    if (rightWordsCount > 0 && i + phrase.Length + 1 < strContent.Length)
                    {
                        rightContent = strContent.Substring(i + phrase.Length + 1);
                        rightWords = ParseWords(rightContent);
                        iStart = 0;
                        while (iStart < rightWords.Count)
                        {
                            rightStr = rightStr + rightWords[iStart] + " ";
                            iStart += 1;
                            if (iStart == rightWordsCount) break;
                        }
                        rightStr = rightStr.Trim();

                    }
                    else
                        rightStr = "";
                    if (rightStr.Length > 0 || leftStr.Length > 0)
                    {
                        drNew = dtResults.Rows.Add();
                        drNew["CorpusID"] = dr["CorpusID"];
                        drNew["Title"] = dr["Title"];
                        drNew["left"] = leftStr;
                        drNew["right"] = rightStr;
                        drNew["match"] = phrase;
                        drNew["OriginalText"] = dr["OriginalText"];

                    }

                }
            }
            return dtResults;
            //DataTable dtTotals = CaculatePhrase(dtResults);
            //return dtTotals;
        }
        #endregion
        #region Cluster
        /// <summary>
        /// 返回一段语料中的语族
        /// </summary>
        /// <param name="corpusContext">文本</param>
        /// <param name="ClusterLength">词族的长度</param>
        /// <returns></returns>
        public static DataTable GetClusterFromCorpus(string corpusContext, int ClusterLength)
        {
            DataTable dt = new DataTable();
            List<string> words = ParseWords(corpusContext);
            dt.Columns.Add("Cluster", typeof(string));
            dt.Columns.Add("Count", typeof(int));
            DataRow dr;
            DataRow[] drs;
            int i = 0;
            string cluster;
            while (i <= words.Count - ClusterLength)
            {
                cluster = GetPhrase(words, words[i], 0, ClusterLength - 1, i);
                drs = dt.Select("Cluster='" + cluster + "'");
                if (drs.Length > 0)
                {
                    dr = drs[0];
                    dr["Count"] = int.Parse(dr["Count"].ToString()) + 1;
                }
                else
                {
                    dr = dt.Rows.Add();
                    dr["Cluster"] = cluster;
                    dr["Count"] = 1;
                }
                i = i + 1;
            }
            dt.DefaultView.Sort = "Count Desc";
            return dt;

        }
        #endregion
        #region 句子中检索单词
        /// <summary>
        /// 通过要查找的单词获取短语字符串
        /// </summary>
        /// <param name="words">划分好的单词</param>
        /// <param name="findWord">要查询的单词</param>
        /// <param name="leftWords">左边的单词个数</param>
        /// <param name="rightWords">右边的单词个数</param>
        /// <returns></returns>
        private static string GetPhrase(List<string> words, string findWord, int leftWords, int rightWords, int startIndex = 0)
        {
            string phrase = "";
            if (leftWords == 0 && rightWords == 0) return phrase;
            int wordIndex = words.FindIndex(startIndex, word => word == findWord);//要找的两个单词相距单词个数与参数个数相同
            if (wordIndex < 0) return phrase;
            int leftIndex = wordIndex - leftWords;
            int rightIndex = wordIndex + rightWords;
            if (leftIndex < 0) leftIndex = 0;
            if (rightIndex > words.Count - 1) rightIndex = words.Count - 1;
            //if (leftIndex == rightIndex) return phrase;
            while (leftIndex <= rightIndex)
            {
                phrase = phrase + words[leftIndex] + " ";
                leftIndex += 1;
            }
            return phrase.Trim();

        }
        /// <summary>
        /// 对满足条件的语料库进行计算,
        /// 返回五列（CorpusID、Title、left、match、right）
        /// </summary>
        /// <param name="dtCorpus">包含单词的语料库</param>
        /// <param name="findWord">要查找的单词</param>
        /// <param name="leftWords">左边的单词</param>
        /// <param name="rightWords"></param>
        /// <returns></returns>
        public static DataTable GetWordsFromCorpus(DataTable dtCorpus, string findWord, int leftWords, int rightWords)
        {
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("CorpusID", typeof(long));
            dtResults.Columns.Add("Title", typeof(string));
            dtResults.Columns.Add("left", typeof(string));
            dtResults.Columns.Add("match", typeof(string));
            dtResults.Columns.Add("right", typeof(string));
            dtResults.Columns.Add("OriginalText", typeof(string));

            DataTable findResults;
            DataRow drNew;
            foreach (DataRow dr in dtCorpus.Rows)
            {
                findResults = FindWord(findWord, dr["OriginalText"].ToString(), leftWords, rightWords);
                foreach (DataRow drWord in findResults.Rows)
                {
                    drNew = dtResults.Rows.Add();
                    drNew["CorpusID"] = dr["CorpusID"];
                    drNew["Title"] = dr["Title"];
                    drNew["left"] = drWord["left"];
                    drNew["match"] = drWord["match"];
                    drNew["right"] = drWord["right"];
                    drNew["OriginalText"] = dr["OriginalText"];

                }
            }

            return dtResults;
        }
        /// <summary>
        /// 返回包含三列的数据表（left,match,right）
        /// </summary>
        /// <param name="findWord">要查询的单词</param>
        /// <param name="strContent">文本内容</param>
        /// <param name="position">单词的匹配位置；-1：左匹配，0：中间匹配，1：匹配</param>
        /// <param name="wordsCount">显示的其他单词的个数</param>
        /// <returns>DataTable</returns>
        private static DataTable FindWord(string findWord, string strContent, int leftWords, int rightWords)
        {
            string ignore = "[\r\n\t\"]";//需要替换的符号
            strContent = Regex.Replace(strContent, ignore, " ");
            strContent = Regex.Replace(strContent, "\\s{2,}", " ");
            //s{2,} 中的s表示空格，数字2表示两个或以上的空格
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("left", typeof(string));
            dtResults.Columns.Add("match", typeof(string));
            dtResults.Columns.Add("right", typeof(string));
            List<List<string>> results = new List<List<string>>();
            List<string> words = ParseWords(strContent);
            int wordIndex = words.FindIndex(word => word == findWord);//要找的两个单词相距单词个数与参数个数相同
            int leftIndex;
            int rightIndex;
            int start;
            string resultWord;//前面或后面的单词
            DataRow dr = null;
            while (wordIndex > -1)
            {
                dr = dtResults.NewRow();
                if (leftWords > 0)//右匹配，找左边的单词
                {
                    resultWord = "";
                    leftIndex = wordIndex - leftWords;
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
                dr["left"] = resultWord;

                dr["match"] = findWord;
                //处理中间
                if (rightWords > 0)
                {
                    resultWord = "";
                    rightIndex = wordIndex + rightWords;
                    if (rightIndex >= words.Count) rightIndex = words.Count - 1;
                    start = wordIndex + 1;
                    while (start <= rightIndex)
                    {
                        resultWord += " " + words[start];
                        start += 1;
                    }
                    resultWord = resultWord.Trim();
                }
                else
                    resultWord = "";
                //三部分的数组
                dr["right"] = resultWord;
                if (!dr.IsNull("left") || !dr.IsNull("right"))
                    dtResults.Rows.Add(dr);

                if (wordIndex + rightWords + 1 >= words.Count)
                    break;
                wordIndex = words.FindIndex(wordIndex + rightWords + 1, word => word == findWord);
            }
            return dtResults;
        }
        #endregion
        #region 处理句子与单词
        public static List<string> SplitSentences(string strContent)
        {
            string ignore = "[\r\n\t\"]";//需要替换的符号
            strContent = Regex.Replace(strContent, ignore, " ");
            Regex rx = new Regex(@"(\S.+?[.!?])(?=\s+|$)");
            MatchCollection matchs = rx.Matches(strContent);
            List<string> sentence = new List<string>();
            foreach (Match match in matchs)
            {
                sentence.Add(match.Value);

            }
            return sentence;
        }
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
            DataRow[] drs;
            if (splitStr.Length > 0)//LC
            {
                string[] idKeys = Regex.Split(ids, splitStr);
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
            }
            else
            {
                drs = dtCorpusExtends.Select("Types='" + typeName + "' and TitleCN='" + ids + "'");
                if (drs.Length > 0)
                    strTitles += drs[0]["Title"].ToString();
            }

            return strTitles;
        }

        public static DataTable GetCorpusByID(long corpusId, string CorpusName, DataTable dtCorpusExtend)
        {
            DataSet dsCorpus = FSCDLL.DAL.Corpus.GetCorporaByID(corpusId);
            DataTable dtCorpus = dsCorpus.Tables[0];
            DataTable dtResult = dtCorpus.Copy();
            DataRow drCorpus;
            if (dtResult != null)
            {
                if (CorpusName == "NEUAC")
                {
                    dtResult.Columns["TopicID"].ColumnName = "MajorID";
                    dtResult.Columns["GenreID"].ColumnName = "JournalID";
                    dtResult.Columns["LevelID"].ColumnName = "YearID";
                    dtResult.Columns.Add("Major");
                    dtResult.Columns.Add("Journal");
                    dtResult.Columns.Add("Year");
                    drCorpus = dtResult.Rows[0];
                    string Ids = SystemDataExtension.GetString(drCorpus, "MajorID");
                    drCorpus["Major"] = GetTitlesByIDs(dtCorpusExtend, Ids, "Major", "");
                    Ids = SystemDataExtension.GetString(drCorpus, "JournalID");
                    drCorpus["Journal"] = GetTitlesByIDs(dtCorpusExtend, Ids, "Journal", "");
                    Ids = SystemDataExtension.GetString(drCorpus, "YearID");
                    drCorpus["Year"] = GetTitlesByIDs(dtCorpusExtend, Ids, "Year", "");
                    dtResult.AcceptChanges();
                }
                else
                {
                    dtResult.Columns.Add("Topic");
                    dtResult.Columns.Add("Genre");
                    dtResult.Columns.Add("Level");
                    drCorpus = dtResult.Rows[0];
                    string Ids = SystemDataExtension.GetString(drCorpus, "TopicID");
                    drCorpus["Topic"] = GetTitlesByIDs(dtCorpusExtend, Ids, "Topic", ";");
                    Ids = SystemDataExtension.GetString(drCorpus, "GenreID");
                    drCorpus["Genre"] = GetTitlesByIDs(dtCorpusExtend, Ids, "Genre", ";");
                    Ids = SystemDataExtension.GetString(drCorpus, "LevelID");
                    drCorpus["Level"] = GetTitlesByIDs(dtCorpusExtend, Ids, "Level", ";");
                    dtResult.AcceptChanges();
                }
            }
            return dtResult;
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
                    {
                        builder.AppendFormat("{0} like '%{1}{2}{1}%'", fieldName, splitStr, cbl.Items[i].Value);
                    }
                    else
                    {
                        builder.AppendFormat(" or {0} like '%{1}{2}{1}%'", fieldName, splitStr, cbl.Items[i].Value);
                    }
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
        /// 获取DataTable前几条数据
        /// </summary>
        /// <param name="TopItem">前N条数据</param>
        /// <param name="oDT">源DataTable</param>
        /// <returns></returns>
        public static DataTable SelectTopNRowsInDT(int TopItem, DataTable oDT)
        {
            if (oDT.Rows.Count < TopItem)
            {
                return oDT;
            }
            else
            {
                DataTable NewTable = oDT.Clone();
                DataRow[] rows = oDT.Select("1=1");
                for (int i = 0; i < TopItem; i++)
                {
                    NewTable.ImportRow((DataRow)rows[i]);
                }
                return NewTable;
            }
        }

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
                DataRow dr = dt.Rows[i];
                string colName = dt.Columns[0].ColumnName;
                string drName = FSCDLL.DAL.SystemDataExtension.GetString(dr, colName);
                if (drName == string.Empty)
                {
                    drName = "Col" + i;
                }

                dtResult.Columns.Add(drName);
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


        public static void ComBindDT(DataTable dtSource, ref DataTable dtResult, string queryCol, string computeCol)
        {
            if (dtSource != null)
            {
                foreach (DataRow dr in dtSource.Rows)
                {
                    string queryValue = SystemDataExtension.GetString(dr, queryCol);
                    string rowFilter = string.Format("{0} = '{1}'", queryCol, queryValue);
                    DataRow[] drs = dtResult.Select(rowFilter);
                    if (drs.Length > 0)
                    {
                        DataRow drResult = drs[0];
                        drResult.BeginEdit();
                        drResult[computeCol] = SystemDataExtension.GetInt32(dr, computeCol) + SystemDataExtension.GetInt32(drResult, computeCol);
                        drResult.EndEdit();
                        drResult.AcceptChanges();
                    }
                    else
                    {
                        DataRow drResult = dtResult.Rows.Add();
                        drResult.BeginEdit();
                        drResult[queryCol] = SystemDataExtension.GetString(dr, queryCol);
                        drResult[computeCol] = SystemDataExtension.GetInt32(dr, computeCol);
                        drResult.EndEdit();
                        drResult.AcceptChanges();
                    }
                }
                dtResult.AcceptChanges();
            }
        }
        #endregion DataTable方法
    }
}
