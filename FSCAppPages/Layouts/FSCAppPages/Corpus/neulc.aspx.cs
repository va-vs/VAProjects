﻿using System;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data;
using System.Web.UI;
using System.Web;
using System.Text.RegularExpressions;

using System.Text;
using lemmatizerDLL;
using Microsoft.SharePoint;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{
    public partial class neulc : LayoutsPageBase
    {

        #region 0 公用
        public ArrayList CiLib;
        //数据分隔符，用来分隔外键的ID值
        private const string split = ";";
        #region 公用事件
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            muNeulc.MenuItemClick += muNeulc_MenuItemClick;


            btnSubmitforCorpus.Click += BtnSubmitforCorpus_Click;

            if (!IsPostBack)
            {

                InitQueryControls();
                ClearQueryControls();
                muNeulc.Items[0].Selected = true;
                Titlelb.Text = "> " + muNeulc.SelectedItem.Text;
            }

        }


        /// <summary>
        /// 页面提醒
        /// </summary>
        /// <param name="info"></param>
        /// <param name="p"></param>
        private static void PageAlert(string info, Page p)
        {
            string script = string.Format("<script>alert('{0}')</script>", info);
            p.ClientScript.RegisterStartupScript(p.GetType(), "", script);
        }
        #endregion
        #region 公用方法        /// <summary>
        /// 去掉文件名中的特殊符号
        /// </summary>
        /// <param name="fileName">原有文件名称</param>
        /// <returns></returns>
        private string GetSimpleFileName(string fileName)
        {
            string retDate = Regex.Replace(fileName, @"[.#：]", "").TrimEnd('-');
            return retDate;

        }        /// <summary>
        /// 文本中单词个数统计
        /// </summary>
        /// <param name="text">要计算的文本</param>
        private int getWordSum(string text)
        {
            string textbasic = text;
            char[] basictemp = text.ToCharArray();
            int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chend = Convert.ToInt32("9fff", 16);
            foreach (char c in basictemp)
            {
                if (' ' != c)
                {
                    string temp = c.ToString();
                    int firstcode = char.ConvertToUtf32(temp, 0);
                    if (firstcode >= chfrom && firstcode <= chend)
                    {
                        textbasic = textbasic.Replace(c, ' ');
                    }
                }
            }

            char[] ch = new char[] { ' ', ',', '?', '!', '(', ')', '\n' };
            string[] stemp = textbasic.Split(ch, StringSplitOptions.RemoveEmptyEntries);

            return stemp.Length;
        }
        /// <summary>
        /// 计算两个时间变量的时间差
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>时间差，格式：{0}天{1}时{2}分{3}秒{4}毫秒</returns>
        private static string TimeSpend(DateTime startTime, DateTime endTime)
        {
            TimeSpan ts = endTime - startTime;
            string rtime = string.Format("{0}天{1}时{2}分{3}秒{4}毫秒", ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            return rtime;
        }

        #endregion 公用方法


        #region 这部分代码需要在Lemme的DLL重写后合并到Dll的方法类中
        /// <summary>
        /// 输出单词等级颜色示例
        /// </summary>
        /// <param name="cngrade">级别标记</param>
        /// <param name="tags">级别号</param>
        /// <param name="wordCount">词频：该级别词汇量</param>
        /// <param name="frequency">todo: describe frequency parameter on GetLegend</param>
        /// <returns>图例Html字符串</returns>
        public string GetLegend(string cngrade, int frequency, int wordCount)
        {
            //****************单词分级cngrade颜色定义*********************/
            //-2:忽略处理的词汇
            //-1:无法在基础词汇表中找到其原型,即单词属于无法处理词汇
            //0:不确定词汇级别,即单词属于超纲词汇
            //1:单词属于高中大纲词汇
            //2:单词属于基本要求词汇
            //3:单词属于较高要求词汇
            //4:单词属于更高要求词汇
            string[] strColors = new string[7] { "grey", "orange", "red", "indigo", "blue", "green", "yellow" };
            string[] strENGrades = new string[7] { "UN", "C1", "C2", "A1", "A2", "B1", "B2" };//级别英文简称，主要用于样式表对应
            string[] strCNGrades = new string[7] { "忽略处理", "未处理", "超纲词汇", "高中大纲", "基本要求", "较高要求", "更高要求" };//级别，主要用于显示文字
            int gradeindex = Array.IndexOf(strCNGrades, cngrade);
            decimal percent = Math.Round(((decimal)frequency / wordCount) * 100, 4);
            string colorstr = string.Format("<dt class='it-chart-dt' data-grade='{0}' onclick='HighLightthis(this)'>{1}</dt>", strENGrades[gradeindex], cngrade);
            colorstr += string.Format("<dd class='it-chart-dd' data-grade='{0}' onclick='HighLightthis(this)'>", strENGrades[gradeindex]);
            colorstr += string.Format("<div class='it-chart-bar' style='background-color: {0}; width:{1}%;'></div>", strColors[gradeindex], percent);
            colorstr += string.Format("<div class='it-chart-label'>{0}({1}%)</div>", frequency, percent.ToString("0.00"));
            colorstr += "</dd>";
            return colorstr;
        }


        public StringBuilder GetCopusContext(IEnumerable<List<string>> showWordsList)
        {
            StringBuilder sb = new StringBuilder();
            string[] strENGrades = new string[7] { "UN", "C1", "C2", "A1", "A2", "B1", "B2" };//级别英文简称，主要用于样式表对应
            //string divString = string.Empty;
            //int i = 0;//计数器
            try
            {
                foreach (List<string> sList in showWordsList)
                {

                    string sword = sList[0]; //文章中出现的单词
                    int stags = int.Parse(sList[1]); //文章中单词对应的级别序号-1,1,2,3...
                    string className = "";
                    if (stags < 2)
                    {
                        className = string.Format("RB {0} {1}", strENGrades[stags + 2], sword);
                    }
                    else
                    {
                        className = string.Format("RB {0} {1}", strENGrades[stags - 2], sword);
                    }
                    //if (stags > maxIndex + 4 || stags == 0) //没有选择的级别或者没有确定级别的基础词汇,即超纲词汇
                    //{
                    //    className = string.Format("RB {0} {1}", strENGrades[stags], sword);
                    //}
                    //else //不在基础词汇表中或者是已经指定级别的词汇
                    //{
                    //    if (stags >= 5)
                    //        className = string.Format("RB {0} {1}", strENGrades[stags - 4], sword);
                    //    else
                    //        className = string.Format("RB {0} {1}", strENGrades[stags], sword);
                    //}
                    sb.AppendFormat("<span class='{0}'> {1}</span>", className, sword);
                }
            }
            catch (Exception ex)
            {

                lbErr.Text = ex.ToString();
            }
            return sb;
        }
        #endregion 这部分代码需要在Lemme的DLL重写后合并到Dll的方法类中

        #endregion 0 公用

        #region 1 顶部工具栏

        #region 顶部工具栏事件

        /// <summary>
        /// 模块菜单点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void muNeulc_MenuItemClick(object sender, MenuEventArgs e)
        {
            switch (muNeulc.SelectedValue)
            {
                case "1"://Concordance
                    {
                        mvNeulc.ActiveViewIndex = 1;
                        break;
                    }
                case "2"://Collocate
                    {
                        mvNeulc.ActiveViewIndex = 2;
                        break;
                    }
                case "3"://WordList
                    {
                        mvNeulc.ActiveViewIndex = 3;
                        inputDiv.Visible = true;
                        outputDiv.Visible = false;
                        mvNeulc.ActiveViewIndex = 0;
                        rbltxtFrom.SelectedValue = "0";
                        lemmanew.Click += lemmanew_Click;
                        btnBackLemma.Click += BtnBackLemma_Click;
                        btnCloseLemma.Click += BtnCloseLemma_Click;
                        rbltxtFrom.SelectedIndexChanged += RbltxtFrom_SelectedIndexChanged;
                        btnQueryforWordlist.Click += BtnQueryforWordlist_Click;
                        string WordsFile = GetDbPath() + "words/AllWords.txt";
                        CiLib = WordBLL.cibiaoku(WordsFile);
                        break;
                    }
                case "4"://Cluster
                    {
                        mvNeulc.ActiveViewIndex = 4;
                        break;
                    }
                default://Corpus
                    mvNeulc.ActiveViewIndex = 0;
                    InitQueryControls();
                    ClearQueryControls();
                    break;
            }
            Titlelb.Text = "> " + muNeulc.SelectedItem.Text;
        }

        #endregion

        #region 顶部工具栏方法

        #endregion

        #endregion

        #region 2 Corpus

        #region Corpus事件        /// <summary>
        /// 检索大库，生成关键小库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmitforCorpus_Click(object sender, EventArgs e)
        {
            try
            {
                string[] strResult = GetSelectQuery();
                string result0 = strResult[0];
                if (result0 != "1;1;1")//不是所有的筛选项都被筛选了
                {
                    //string[] strs = result0.Split(';');
                    //if (strs[0] != "1")//Grade
                    //{
                    //    lbErr.Text = "你尚未选择筛选的Grade";
                    //    return;
                    //}

                    //if (strs[1] != "1")//Genner
                    //{
                    //    lbErr.Text = "你尚未选择筛选的Genre";
                    //    return;
                    //}

                    //if (strs[2] != "1")//Topic
                    //{
                    //    lbErr.Text = "你尚未选择筛选的Topic";
                    //    return;
                    //}
                    lbErr.Text = "Grade、Genre、Topic中每项都至少要选择一个条目！";
                    return;
                }
                else
                {
                    string newQueryStr = strResult[1];
                    DataSet dsAll = FSCDLL.DAL.Corpus.GetCorpus();
                    BuildTable(dsAll, cblGrade, "GradeID", tbforGrade);
                    //if (ViewState["filterExp"] != null)//已有检索历史
                    //{
                    //    string oldQueryStr = ViewState["filterExp"].ToString();
                    //    if (oldQueryStr != newQueryStr)//检索条件改变了，才进行重新检索
                    //    {
                    //        ViewState["filterExp"] = newQueryStr;
                    //        ViewState["dsCorpus"] = null;//清空检索
                    //        QueryCorpus();//检索语料库
                    //    }
                    //}
                    //else//尚未检索过
                    //{
                    //    ViewState["filterExp"] = newQueryStr;//清空旧的检索字符串
                    //    ViewState["dsCorpus"] = null;//清空检索
                    //    QueryCorpus();
                    //}
                }
            }
            catch (Exception ex)
            {

                lbErr.Text = ex.ToString();
            }
        }





        #endregion

        #region Corpus方法


        private DataTable BuildDTSummary(DataSet dsCorpus, string fkid, CheckBoxList cbl)
        {
            DataTable dt = dsCorpus.Tables[0];
            DataTable dtSummary = new DataTable();
            dtSummary.Columns.Add("Summary");
            dtSummary.Columns.Add("texts");
            dtSummary.Columns.Add("types");
            dtSummary.Columns.Add("tokens");
            dtSummary.Columns.Add("TTR");
            dtSummary.Columns.Add("wAvglength");
            dtSummary.Columns.Add("wstandard");
            dtSummary.Columns.Add("pAvglength");
            dtSummary.Columns.Add("pstandard");
            List<string> listFKs = new List<string> { "GradeID", "TopicID", "GenreID" };
            listFKs.Remove(fkid);
            foreach (string fk in listFKs)
            {
                dtSummary.Columns.Add(fk);
            }
            DataRow drSummary = dtSummary.NewRow();

            drSummary[0] = " ";
            drSummary[1] = "texts";
            drSummary[2] = "types";
            drSummary[3] = "tokens";
            drSummary[4] = "TTR";
            drSummary[5] = "mean word length (in characters)";
            drSummary[6] = "mean word length standard deviation";
            drSummary[7] = "mean in words";
            drSummary[8] = "standard deviation";
            drSummary[9] = listFKs[0].Replace("ID", "s");
            drSummary[10] = listFKs[1].Replace("ID", "s");
            dtSummary.Rows.Add(drSummary);
            for (int i = 0; i < cbl.Items.Count; i++)
            {
                if (cbl.Items[i].Selected)
                {
                    string selectValue = string.Format(";{0};", cbl.Items[i].Value);
                    drSummary = dtSummary.NewRow();
                    drSummary["Summary"] = cbl.Items[i].Text;//统计项，比如年级中的F1，S2...
                    //用dataview的方法实现筛选，dt为内存表
                    DataView dv = dt.DefaultView;
                    string filterStr = string.Format("{0} = '{1}'", fkid, selectValue);
                    dv.RowFilter = filterStr;
                    //将查得的数据转换为DataTable
                    DataTable dtSelect = dv.ToTable();
                    if (dtSelect.Rows.Count > 0)
                    {
                        List<string> fk1 = new List<string>();
                        List<string> fk2 = new List<string>();
                        List<string> listWords = new List<string>();
                        List<int> listParaLength = new List<int>();
                        drSummary["texts"] = dtSelect.Rows.Count;//筛选后的语料条数
                        int rowIndex = 0;
                        foreach (DataRow dr in dtSelect.Rows)
                        {
                            string para = dr["OriginalText"].ToString();//本条语料的内容
                            Dictionary<int, List<string>> dictPara = Common.ParseSentences(para);//对本类别下的语篇进行句子拆分，生成句子序列和句子对应的单词词组
                            foreach (int key in dictPara.Keys)
                            {
                                listParaLength.Add(dictPara[key].Count);
                                //listWords = listWords.Concat(dictPara[key]).ToList<string>(); //保留重复项，合并词汇数组
                            }
                            string strFks = dr[listFKs[0]].ToString();
                            if (!fk1.Contains(strFks))
                                fk1.Add(strFks);
                            strFks = dr[listFKs[1]].ToString();
                            if (!fk2.Contains(strFks))
                                fk2.Add(strFks);
                            rowIndex++;

                        }

                        double avg = listParaLength.Average();
                        drSummary["pAvglength"] = avg.ToString("0.00");//平均句长
                        //  计算各数值与平均数的差值的平方，然后求和
                        double sum = listParaLength.Sum(d => Math.Pow(d - avg, 2));
                        //  除以数量，然后开方
                        double standard = Math.Sqrt(sum / listParaLength.Count);
                        drSummary["pstandard"] = standard.ToString("0.00");//句长标准差


                        int totalWords = listWords.Count;
                        List<int> listWordLength = new List<int>();
                        foreach (string wd in listWords)
                        {
                            listWordLength.Add(wd.Length);
                        }
                        avg = listWordLength.Average();
                        drSummary["wAvglength"] = avg.ToString("0.00");//平均词长
                        sum = listWordLength.Sum(d => Math.Pow(d - avg, 2));
                        standard = Math.Sqrt(sum / totalWords);
                        drSummary["wstandard"] = standard.ToString("0.00");//词长标准差


                        drSummary["tokens"] = totalWords;//单词总数

                        listWords = listWords.Distinct().ToList();
                        int distinctWords = listWords.Count;
                        drSummary["types"] = distinctWords;//去重复后单词数
                        double percent = Convert.ToDouble(distinctWords) / Convert.ToDouble(totalWords);
                        drSummary["TTR"] = percent.ToString("0.00%");

                        drSummary[9] = fk1.Distinct().ToList().Count;
                        drSummary[10] = fk2.Distinct().ToList().Count;

                    }
                    else//该类别下没有语篇
                    {
                        drSummary["texts"] = 0;
                        for (int k = 2; k < dtSummary.Columns.Count; k++)
                        {
                            drSummary[k] = " - ";
                        }
                    }
                    dtSummary.Rows.Add(drSummary);
                }
            }
            return dtSummary;
        }

        private void BuildTable(DataSet dsCorpus, CheckBoxList cbl, string fkid, Table tbSummary)
        {
            DataTable dt = dsCorpus.Tables[0];
            DataTable dtSummary = BuildDTSummary(dsCorpus, fkid, cbl);
            DataTable dtResult = Common.TranspositionDT(dtSummary);
            List<string> listValues = new List<string>();
            List<string> listTexts = new List<string>();
            List<Dictionary<int, List<string>>> listDict = new List<Dictionary<int, List<string>>>();
            //表头行
            DataRow drHead = dtResult.Rows[0];
            TableHeaderRow thr = new TableHeaderRow();
            TableHeaderCell thc = new TableHeaderCell() { Text = drHead[0].ToString() };
            thr.Cells.Add(thc);
            for (int i = 1; i < dtResult.Columns.Count; i++)
            {
                thc = new TableHeaderCell() { Text = drHead[i].ToString() };
                thr.Cells.Add(thc);
            }

            tbSummary.Rows.Add(thr);


            for (int i = 1; i < dtResult.Rows.Count; i++)
            {
                DataRow dr = dtResult.Rows[i];
                TableRow tr = new TableRow();
                thc = new TableHeaderCell() { Text = dr[0].ToString() };
                tr.Cells.Add(thc);
                for (int j = 1; j < dtResult.Columns.Count; j++)
                {
                    TableCell tc = new TableCell();
                    tc = new TableHeaderCell() { Text = dr[j].ToString() };
                    tr.Cells.Add(tc);
                }
                tbSummary.Rows.Add(tr);
            }

        }        /// <summary>
        /// 根据条件筛选大库，生成关键小库的数据集
        /// </summary>
        private void QueryCorpus()
        {
            DataTable dtAll;
            DataSet dsAll = FSCDLL.DAL.Corpus.GetCorpus();
            if (ViewState["dtAll"] == null)
            {
                dtAll = dsAll.Tables[0];
                ViewState["dtAll"] = dtAll;
            }
            else
            {
                dtAll = (DataTable)ViewState["dtAll"];
            }

            string filterExpression = ViewState["filterExp"].ToString();
            DataRow[] drs = dtAll.Select(filterExpression);
            DataTable dtResult = dtAll.Clone();
            DataSet dsResult = new DataSet();
            dsResult.Tables.Add(dtResult);
            dsResult.Merge(drs);
            ViewState["dsCorpus"] = dsResult;

            BuildTable(dsAll, cblGrade, "GradeID", tbforGrade);
            //BuildTable(dsAll, cblTopic, "TopicID", tbforTopic);
            //BuildTable(dsAll, cblGenre, "GenreID", tbforGenre);
            divforCorpusResult.Visible = true;
        }        /// <summary>
        /// 构造检索字符串，用于检索大库
        /// </summary>
        /// <returns></returns>
        private string[] GetSelectQuery()
        {
            //三个筛选条件：grade、topic、genre
            string strQuery = "";
            string strResult = "";
            if (cblGrade.SelectedIndex >= 0)
            {
                strQuery = Common.GetQueryString(cblGrade, "GradeID", split);
                strResult = "1;";
            }
            else
            {
                strResult = "0;";
            }
            string strSql = "";
            if (cblGenre.SelectedIndex >= 0)
            {
                strSql = Common.GetQueryString(cblGenre, "GenreID", split);
                strResult += "1;";
            }
            else
            {
                strResult += "0;";
            }

            if (strQuery == "")
            {
                strQuery = strSql;
            }
            else
            {
                if (strQuery.IndexOf("and") > 0)
                {
                    strQuery = string.Format("{0} and ({1})", strQuery, strSql);
                }
                else
                {
                    strQuery = string.Format("({0}) and ({1})", strQuery, strSql);
                }
            }
            strSql = "";
            if (cblTopic.SelectedIndex >= 0)
            {
                strSql = Common.GetQueryString(cblTopic, "TopicID", split);
                strResult += "1";
            }
            else
            {
                strResult += "0";
            }

            if (strQuery == "")
            {
                strQuery = strSql;
            }
            else
            {
                if (strQuery.IndexOf("and") > 0)
                {
                    strQuery = string.Format("{0} and ({1})", strQuery, strSql);
                }
                else
                {
                    strQuery = string.Format("({0}) and ({1})", strQuery, strSql);
                }
            }
            string[] strResults = new string[] { strResult, strQuery };
            return strResults;
        }        /// <summary>
        /// 初始化检索控件控件
        /// </summary>
        private void InitQueryControls()
        {
            //话题
            string types = "Topic";
            DataSet ds = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);
            cblTopic.Items.Clear();
            cblTopic.DataSource = ds.Tables[0].Copy();
            cblTopic.DataTextField = "Title";
            cblTopic.DataValueField = "ItemID";
            cblTopic.DataBind();

            //文体
            types = "Genre";
            DataSet dsGenre = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);
            cblGenre.Items.Clear();
            cblGenre.DataSource = dsGenre.Tables[0].Copy();
            cblGenre.DataTextField = "Title";
            cblGenre.DataValueField = "ItemID";
            cblGenre.DataBind();

            //年级
            types = "Grade";
            DataSet dsGrade = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);
            cblGrade.Items.Clear();
            cblGrade.DataSource = dsGrade.Tables[0].Copy();
            cblGrade.DataTextField = "Title";
            cblGrade.DataValueField = "ItemID";
            cblGrade.DataBind();
        }

        /// <summary>
        /// 未检索语料时先清空原有的控件
        /// </summary>
        private void ClearQueryControls()
        {
            foreach (ListItem itm in cblTopic.Items)
            {
                itm.Selected = false;
            }

            foreach (ListItem itm in cblGenre.Items)
            {
                itm.Selected = false;
            }

            foreach (ListItem itm in cblGrade.Items)
            {
                itm.Selected = false;
            }
            divforCorpusResult.Visible = false;
        }

        #endregion Corpus方法

        #endregion

        #region 3 Concordance

        #region Concordance事件

        #endregion

        #region Concordance方法

        #endregion

        #endregion

        #region 4 Collocate

        #region Collocate事件

        #endregion

        #region Collocate方法

        #endregion

        #endregion

        #region 5 Cluster

        #region Cluster事件

        #endregion

        #region Cluster方法

        #endregion

        #endregion

        #region 6 WordList

        #region WordList事件
        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void backBtn_OnClickBtn_OnClick(object sender, EventArgs e)
        {
            outputDiv.Visible = false;
            inputDiv.Visible = true;
        }


        /// <summary>
        /// 清空按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearBtn_OnClick(object sender, EventArgs e)
        {
            txtcontent.Value = "";
        }        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void closeBtn_OnClick(object sender, EventArgs e)
        {
            outputDiv.Visible = false;
            inputDiv.Visible = true;
            txtcontent.Value = "";//清空正文文本
            for (int i = 0; i < rbVBS.Items.Count; i++)//清除词表选择
            {
                if (rbVBS.Items[i].Selected)
                {
                    rbVBS.Items[i].Selected = false;
                }
            }
            txt_Title.Value = "";//清空正文标题
            lemmanew.Enabled = true;
            outputDiv.Visible = false;
            inputDiv.Visible = true;
        }        /// <summary>
        /// 筛选词汇等级选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cblist_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (CheckBoxList cblist = new CheckBoxList())
            {
                string clickedItem = Request.Form["__EVENTTARGET"];//得到用户点击的是哪个
                if (clickedItem.Length > clickedItem.LastIndexOf("$", StringComparison.Ordinal) + 1)
                {
                    clickedItem = clickedItem.Substring(clickedItem.LastIndexOf("$") + 1);//进行拆分处理
                    int thisIndex = int.Parse(clickedItem);
                    if (cblist.Items[thisIndex].Selected)
                    {
                        for (int i = 0; i <= thisIndex; i++)
                        {
                            cblist.Items[i].Selected = true;
                        }
                        for (int i = thisIndex + 1; i < cblist.Items.Count; i++)
                        {
                            cblist.Items[i].Selected = false;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < thisIndex; i++)
                        {
                            cblist.Items[i].Selected = true;
                        }
                        for (int i = thisIndex; i < cblist.Items.Count; i++)
                        {
                            cblist.Items[i].Selected = false;
                        }
                    }
                }
            }

        }

        private void RbltxtFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbltxtFrom.SelectedValue == "0")
            {
                divfromshuru.Visible = false;
                divTexts.Visible = true;
                divFromCorpus.Visible = true;
                txtKeyWordsforWordlist.Value = "";
            }
            else
            {
                divfromshuru.Visible = true;
                divTexts.Visible = true;
                username.Value = "";
                txt_Title.Value = "";
                divFromCorpus.Visible = false;
            }
            txtcontent.InnerText = "";
        }

        /// <summary>
        /// 按关键字检索生成WordList备选语料列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        private void BtnQueryforWordlist_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKeyWordsforWordlist.Value.Trim()))
            {
                PageAlert("请先输入检索关键词！", this);
                txtKeyWordsforWordlist.Focus();
                return;
            }
            string keyWords = txtKeyWordsforWordlist.Value.Trim();
            DataSet dsCorpus = (DataSet)ViewState["dsCorpus"];
            DataTable dtCorpus = dsCorpus.Tables[0];
            string strSql = string.Format("Title like '%{0}%' or Source like '%{0}%' or OriginalText like '%{0}%' or CodedText like '%{0}%'", keyWords);
            DataRow[] drs = dtCorpus.Select(strSql);
            if (drs.Length > 0)
            {
                DataTable dtCorpusforWordList = dtCorpus.Clone();
                foreach (var dr in drs)
                {
                    dtCorpusforWordList.Rows.Add(dr.ItemArray);
                }
                gvCorpusforWordList.DataSource = dtCorpusforWordList;
                gvCorpusforWordList.DataBind();
            }
            else
            {
                lbErr.Text = string.Format("未检索到与关键词'{0}'相关的语料，请换个关键词重试！", keyWords);
                txtKeyWordsforWordlist.Focus();
            }
        }

        /// <summary>
        /// 关闭文本彩色标记输出，返回到标记前界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        private void BtnCloseLemma_Click(object sender, EventArgs e)
        {
            username.Value = "";

            outputDiv.Visible = false;
            inputDiv.Visible = true;
        }

        private void BtnBackLemma_Click(object sender, EventArgs e)
        {
            outputDiv.Visible = false;
            inputDiv.Visible = true;
        }


        /// <summary>
        /// Lemma操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lemmanew_Click(object sender, EventArgs e)
        {
            lemmanew.Enabled = false;
            #region 0 变量定义与表单校验
            string titleStr;//标题
            string nameStr;//用户名
            string txtStr = txtcontent.Value.Trim();//正文文本

            //检验文档标题、用户名、正文是否输入完成
            if (string.IsNullOrEmpty(txt_Title.Value) || txt_Title.Value == "Type the title or click to choose it")//标题为空或者为文本框提示值,即未输入标题
            {
                PageAlert("你还未选择或输入文档标题!", this);
                txt_Title.Focus();
                lemmanew.Enabled = true;
                return;
            }
            else
            {
                titleStr = txt_Title.Value;//标题
            }
            if (string.IsNullOrEmpty(username.Value))//用户名为空,即未输入有效用户名
            {
                PageAlert("请先输入你的姓名，本系统不支持匿名操作！", this);
                username.Focus();
                lemmanew.Enabled = true;
                return;
            }
            else
            {
                nameStr = username.Value;//用户名
            }
            if (string.IsNullOrEmpty(txtcontent.Value)) //处理的文本还未输入
            {
                PageAlert("你还未输入或导入需要处理的文本,请确认后再试！", this);
                txtcontent.Focus();
                lemmanew.Enabled = true;
                return;
            }
            else
            {
                string regEx = @"((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";
                txtStr = Regex.Replace(txtcontent.Value, regEx, ";");//正则表达式排除文中的网址
            }
            #endregion

            #region 1 过滤文本判断是否包含有英文单词
            string ignoreWordsFile = GetDbPath() + "words/ignoreWords.txt";
            string ordinalWordsFile = GetDbPath() + "words/OrdinalWords.txt";
            string symbolFile = GetDbPath() + "words/symbol.txt";

            var txtlist = TextInput.ArticleToList(txtStr, ignoreWordsFile, ordinalWordsFile, symbolFile);//文本转化为字符串数组,将需要处理的单词存到数组中
            if (txtlist.Count == 0)//文本中不包含有英文单词
            {
                PageAlert("文本中不包含需要处理的英文单词！", this);
                txtcontent.Focus();
                lemmanew.Enabled = true;
                return;
            }
            #endregion

            #region 2 参照词库选择
            int maxIndex = 4;
            #endregion

            #region 3 保存要处理的文本
            SPUser currentUser = SPContext.Current.Web.CurrentUser;

            string spName = currentUser.Name;
            if (nameStr != spName)
            {
                nameStr = string.Format("{0}_{1}", nameStr, spName);
            }
            titleStr = TextInput.FilterSpecial(titleStr, "");
            string filePath = GetDbPath() + @"export/";//txt文件保存的路径
            string nowStr = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now);//时间格式字符串：年月日时分秒4位毫秒
            string fileTitle = string.Format("{0}({1}){2}.txt", titleStr, nameStr, nowStr); ;//文章标题+ _ + 处理人姓名 + 处理人所属院校（登录名）+ 当前时间
            TextInput.FileWrite(fileTitle, txtStr, filePath);//将即将处理的文本保存到服务器上的指定目录中;

            inputDiv.Visible = false;
            #endregion

            #region 4 单词还原
            string fileName = GetDbPath() + "words/AllWords.txt";//包含原型与变型以及对应等级的词汇表
            int isEurope = 0;
            //if (ckEurope.Checked)
            //{
            //    isEurope = 1;
            //}
            Dictionary<int, object> allwordsList = WordBLL.SearchWordsWithTxt(txtlist, fileName, isEurope);//对词汇列表进行比对还原和级别确认，输出三个数据集：1、文本词汇对应级别，2、超纲词汇对应词频，3、处理过的单词原型对应级别
            #endregion

            #region 5 WordList和结果输出
            if (allwordsList.Count > 0)
            {
                var showWordsList = (List<List<string>>)allwordsList[0];//文本处理后包含的级别及每个级别词频的列表集合
                DataTable dt = OutputResult.InitWordsAnalysisTable(showWordsList, maxIndex, symbolFile);

                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    sb.Append(GetLegend(dr[0].ToString(), int.Parse(dr[1].ToString()), txtlist.Count));
                }

                dlChart.InnerHtml = sb.ToString();
                divContext.InnerHtml = GetCopusContext(showWordsList).ToString();
                outputDiv.Visible = true;
            }
            lemmanew.Enabled = true;
            #endregion
        }
        #endregion WordList事件

        #region WordList方法

        private string GetDbPath()
        {
            string txtPath = Server.MapPath("");
            txtPath = txtPath.Substring(0, txtPath.IndexOf("\\layouts", StringComparison.Ordinal)) + @"\layouts\db\";
            return txtPath;
        }

        #endregion WordList方法

        #endregion
    }
}
