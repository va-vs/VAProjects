using System;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Text;
using lemmatizerDLL;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.DataVisualization;
using System.Web.UI.DataVisualization.Charting;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{
    public partial class neulc : LayoutsPageBase
    {
        #region 0 公用
        //public ArrayList CiLib;
        //数据分隔符，用来分隔外键的ID值
        private const string splitStr = ";";
        #region 公用事件
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //导航按钮点击事件
            muNeulc.MenuItemClick += muNeulc_MenuItemClick;

            //Concordance提交按钮点击事件
            btnSubmitforCorpus.Click += BtnSubmitforCorpus_Click;

            //WordList控件事件绑定
            btnBacktoQuery.Click += BtnBacktoQuery_Click;
            lemmanew.Click += lemmanew_Click;//WordList提交按钮点击事件
            btnBackLemma.Click += BtnBackLemma_Click;
            btnCloseLemma.Click += BtnCloseLemma_Click;
            rbltxtFrom.SelectedIndexChanged += RbltxtFrom_SelectedIndexChanged;
            btnQueryforWordlist.Click += BtnQueryforWordlist_Click;
            gvCorpusforWordList.RowDataBound += GvCorpusforWordList_RowDataBound;
            gvCorpusforWordList.PageIndexChanging += GvCorpusforWordList_PageIndexChanging;
            btnLemmaAll.Click += BtnLemmaAll_Click;
            clearBtn.Click += clearBtn_OnClick;

            //Concordance 内的控件事件绑定
            btnSubmitConcordance.Click += BtnSubmitConcordance_Click;

            btnReConc.Click += BtnReConc_Click;
            gvConcordance.RowDataBound += GvConcordance_RowDataBound;
            gvConcordance.PageIndexChanging += GvConcordance_PageIndexChanging;
            gvConcordance.RowCommand += GvConcordance_RowCommand;

            //gvConcComputed.RowDataBound += GvConcComputed_RowDataBound;
            //gvConcComputed.PageIndexChanging += GvConcComputed_PageIndexChanging;
            //gvConcComputed.RowCommand += GvConcComputed_RowCommand;
            //btnCloseConc.Click += BtnCloseConc_Click;

            //Collocate 内的控件事件绑定
            btnSubmitCollocate.Click += BtnSubmitCollocate_Click;

            btnReColl.Click += BtnReColl_Click;
            gvCollocate.RowDataBound += GvCollocate_RowDataBound;
            gvCollocate.PageIndexChanging += GvCollocate_PageIndexChanging;
            gvCollocate.RowCommand += GvCollocate_RowCommand;

            gvCollComputed.RowCommand += GvCollComputed_RowCommand;
            gvCollComputed.PageIndexChanging += GvCollComputed_PageIndexChanging;
            gvCollComputed.RowDataBound += GvCollComputed_RowDataBound;
            btnCloseColl.Click += BtnCloseColl_Click;

            //Compare提交按钮点击事件
            btnCompared.Click += BtnCompared_Click;
            InitMenu();
            //页面加载
            if (!IsPostBack)
            {
                InitQueryControls();
                ClearQueryControls();
                muNeulc.Items[0].Selected = true;
                Titlelb.Text = "> " + muNeulc.SelectedItem.Text;

                mvNeulc.ActiveViewIndex = 0;

                //WordList 变量与状态
                rbltxtFrom.SelectedValue = "0";
                hdftxtFrom.Value = "0";
                divfromshuru.Visible = true;
                divTexts.Visible = true;
                divFromCorpus.Visible = false;
                divOutput.Visible = false;
            }

        }











        #endregion
        #region 公用方法
        private string GetCorpusByUrl()
        {
            string urlp = "NEULC";
            if (Request.QueryString["cp"] != null)
            {
                urlp = Request.QueryString["cp"];
            }
            return urlp;
        }
        /// <summary>
        /// 将表格数据绑定到GridView控件
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="dtSource"></param>
        private void GVBind(GridView gv, DataTable dtSource)
        {
            gv.DataSource = dtSource;
            gv.DataBind();
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
        }        /// <summary>
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
        /// <param name="cnLevel">级别标记</param>
        /// <param name="tags">级别号</param>
        /// <param name="wordCount">词频：该级别词汇量</param>
        /// <param name="frequency">todo: describe frequency parameter on GetLegend</param>
        /// <returns>图例Html字符串</returns>
        public string GetLegend(string cnLevel, int frequency, int wordCount)
        {
            //****************单词分级cnLevel颜色定义*********************/
            //-2:忽略处理的词汇
            //-1:无法在基础词汇表中找到其原型,即单词属于无法处理词汇
            //0:不确定词汇级别,即单词属于超纲词汇
            //1:单词属于高中大纲词汇
            //2:单词属于基本要求词汇
            //3:单词属于较高要求词汇
            //4:单词属于更高要求词汇
            string[] strColors = new string[7] { "grey", "orange", "red", "indigo", "blue", "green", "yellow" };
            string[] strENLevels = new string[7] { "UN", "C1", "C2", "A1", "A2", "B1", "B2" };//级别英文简称，主要用于样式表对应
            string[] strCNLevels = new string[7] { "忽略处理", "未处理", "超纲词汇", "高中大纲", "基本要求", "较高要求", "更高要求" };//级别，主要用于显示文字
            int Levelindex = Array.IndexOf(strCNLevels, cnLevel);
            decimal percent = Math.Round(((decimal)frequency / wordCount) * 100, 4);
            string colorstr = string.Format("<dt class='it-chart-dt' data-Level='{0}' onclick='HighLightthis(this)'>{1}</dt>", strENLevels[Levelindex], cnLevel);
            colorstr += string.Format("<dd class='it-chart-dd' data-Level='{0}' onclick='HighLightthis(this)'>", strENLevels[Levelindex]);
            colorstr += string.Format("<div class='it-chart-bar' style='background-color: {0}; width:{1}%;'></div>", strColors[Levelindex], percent);
            colorstr += string.Format("<div class='it-chart-label'>{0}({1}%)</div>", frequency, percent.ToString("0.00"));
            colorstr += "</dd>";
            return colorstr;
        }


        public StringBuilder GetCopusContext(IEnumerable<List<string>> showWordsList)
        {
            StringBuilder sb = new StringBuilder();
            string[] strENLevels = new string[7] { "UN", "C1", "C2", "A1", "A2", "B1", "B2" };//级别英文简称，主要用于样式表对应
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
                        className = string.Format("RB {0} {1}", strENLevels[stags + 2], sword);
                    }
                    else
                    {
                        className = string.Format("RB {0} {1}", strENLevels[stags - 2], sword);
                    }
                    //if (stags > maxIndex + 4 || stags == 0) //没有选择的级别或者没有确定级别的基础词汇,即超纲词汇
                    //{
                    //    className = string.Format("RB {0} {1}", strENLevels[stags], sword);
                    //}
                    //else //不在基础词汇表中或者是已经指定级别的词汇
                    //{
                    //    if (stags >= 5)
                    //        className = string.Format("RB {0} {1}", strENLevels[stags - 4], sword);
                    //    else
                    //        className = string.Format("RB {0} {1}", strENLevels[stags], sword);
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
            try
            {
                lbErr.Text = "";
                ViewState["filterExp"] = null;
                switch (muNeulc.SelectedValue)
                {
                    case "NEUAC"://NEUAC
                        {
                            mvNeulc.ActiveViewIndex = 1;
                            break;
                        }
                    case "Concordance"://Concordance
                        {
                            mvNeulc.ActiveViewIndex = 1;
                            break;
                        }
                    case "Collocate"://Collocate
                        {
                            mvNeulc.ActiveViewIndex = 2;
                            break;
                        }
                    case "WordList"://WordList
                        {
                            mvNeulc.ActiveViewIndex = 3;
                            break;
                        }
                    case "Cluster"://Cluster
                        {
                            mvNeulc.ActiveViewIndex = 4;
                            break;
                        }
                    case "Compare"://Compare
                        {
                            mvNeulc.ActiveViewIndex = 5;
                            break;
                        }
                    default://Corpus
                        mvNeulc.ActiveViewIndex = 0;
                        InitMenu();
                        break;
                }
                Titlelb.Text = "> " + muNeulc.SelectedItem.Text;
            }
            catch (Exception ex)
            {

                lbErr.Text = ex.ToString();
            }
        }


        #endregion

        #region 顶部工具栏方法

        private void InitMenu()
        {
            string strCorpus = GetCorpusByUrl();
            lbCorpus.Text = strCorpus;
            if (strCorpus == "NEULC")
            {
                divNEULC.Visible = true;
                divNEUAC.Visible = false;
                InitQueryControls(strCorpus);
                ClearQueryControls(strCorpus);
            }
            else
            {
                divNEULC.Visible = false;
                divNEUAC.Visible = true;
            }
        }

        #endregion

        #endregion

        #region 2 Corpus：从整个语料数据库中按条件检索匹配的部分子语料库库

        #region Corpus事件
        /// <summary>
        /// 重新检索语料库（大库筛小库）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBacktoQuery_Click(object sender, EventArgs e)
        {
            divNEULC.Visible = true;
            divforCorpusResult.Visible = false;
        }        /// <summary>
        /// 检索大库，生成关键小库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmitforCorpus_Click(object sender, EventArgs e)
        {

            string[] strResult = GetSelectQuery();
            string result0 = strResult[0];
            if (result0 != "1;1;1")//不是所有的筛选项都被筛选了
            {
                lbErr.Text = "Level、Genre、Topic中每项都至少要选择一个条目！";
                rbltxtFrom.Items[1].Enabled = false;
                return;
            }
            else
            {
                string newQueryStr = strResult[1];
                if (ViewState["filterExp"] != null)//已有检索历史
                {
                    string oldQueryStr = ViewState["filterExp"].ToString();
                    if (oldQueryStr != newQueryStr)//检索条件改变了，才进行重新检索
                    {
                        ViewState["filterExp"] = newQueryStr;
                    }
                }
                else//尚未检索过
                {
                    ViewState["filterExp"] = newQueryStr;//清空旧的检索字符串
                }
                QueryCorpus();//检索语料库
                divforCorpusResult.Visible = true;
                divNEULC.Visible = false;
                rbltxtFrom.Items[1].Enabled = true;//经过检索后，WordList中才可以使用关键词检索语料库文本做WordList
            }

        }

        #endregion Corpus事件

        #region Corpus方法
        private DataTable BuildDTSummary(DataTable dtCorpus, string fkid, CheckBoxList cbl, List<string> listFKs)
        {
            #region 构造汇总表
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

            listFKs.Remove(fkid);
            foreach (string fk in listFKs)
            {
                dtSummary.Columns.Add(fk);
            }
            DataRow drSummary = dtSummary.NewRow();

            drSummary[0] = fkid.Replace("ID", "s");
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
            #endregion 构造汇总表

            for (int i = 0; i < cbl.Items.Count; i++)
            {
                if (cbl.Items[i].Selected)
                {
                    string selectValue = string.Format(";{0};", cbl.Items[i].Value);
                    drSummary = dtSummary.NewRow();
                    drSummary["Summary"] = cbl.Items[i].Text;//统计项，比如年级中的F1，S2...
                    //用dataview的方法实现筛选，dt为内存表
                    DataView dv = dtCorpus.DefaultView;
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
                                foreach (string wd in dictPara[key])
                                {
                                    listWords.Add(wd);//获取单词长度
                                }
                                //listWords = listWords.Concat(dictPara[key]).ToList<string>(); //保留重复项，合并词汇数组
                            }
                            string strFks = dr[listFKs[0]].ToString();
                            if (!fk1.Contains(strFks))
                            {
                                fk1.Add(strFks);
                            }

                            strFks = dr[listFKs[1]].ToString();
                            if (!fk2.Contains(strFks))
                            {
                                fk2.Add(strFks);
                            }

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


        /// <summary>
        /// 显示检索结果表数据
        /// </summary>
        /// <param name="dtCorpus">语料库</param>
        /// <param name="cbl"></param>
        /// <param name="fkid"></param>
        /// <param name="tbSummary"></param>
        private void BuildTable(DataTable dtCorpus, CheckBoxList cbl, string fkid, Table tbSummary, List<string> listFKs)
        {
            DataTable dtSummary = BuildDTSummary(dtCorpus, fkid, cbl, listFKs);
            DataTable dtResult = Common.TranspositionDT(dtSummary);
            List<string> listValues = new List<string>();
            List<string> listTexts = new List<string>();
            List<Dictionary<int, List<string>>> listDict = new List<Dictionary<int, List<string>>>();

            tbSummary.Rows.Clear();//清空原表格内容
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
                    TableCell tc = new TableCell() { Text = dr[j].ToString() };
                    tr.Cells.Add(tc);
                }
                tbSummary.Rows.Add(tr);
            }
        }        /// <summary>
        /// 根据条件筛选大库，生成关键小库的数据集
        /// </summary>
        private void QueryCorpus()
        {
            string filterExpression = ViewState["filterExp"].ToString();
            DataSet dsResult = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterExpression);
            DataTable dtResult = dsResult.Tables[0].Copy();
            List<string> listFKs = new List<string> { "LevelID", "TopicID", "GenreID" };
            BuildTable(dtResult, cblLevel, "LevelID", tbforLevel, listFKs);
            BuildTable(dtResult, cblTopic, "TopicID", tbforTopic, listFKs);
            BuildTable(dtResult, cblGenre, "GenreID", tbforGenre, listFKs);
        }        /// <summary>
        /// 构造检索字符串，用于检索大库
        /// </summary>
        /// <returns></returns>
        private string[] GetSelectQuery(string cpName)
        {
            string strQuery = "";
            string strResult = "";

            if (cpName == "neulc")
            {
                //三个筛选条件：Level、topic、genre
                if (cblLevel.SelectedIndex >= 0)
                {
                    strQuery = Common.GetQueryString(cblLevel, "LevelID", splitStr);
                    strResult = "1;";
                }
                else
                {
                    strResult = "0;";
                }
                string strSql = "";
                if (cblGenre.SelectedIndex >= 0)
                {
                    strSql = Common.GetQueryString(cblGenre, "GenreID", splitStr);
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
                    strSql = Common.GetQueryString(cblTopic, "TopicID", splitStr);
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
            }
            else
            {
                //三个筛选条件：Year、Major、Journal
                if (cblYears.SelectedIndex >= 0)
                {
                    strQuery = Common.GetQueryString(cblYears, "YearID", splitStr);
                    strResult = "1;";
                }
                else
                {
                    strResult = "0;";
                }
                string strSql = "";
                if (cblGenre.SelectedIndex >= 0)
                {
                    strSql = Common.GetQueryString(cblGenre, "GenreID", splitStr);
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
                    strSql = Common.GetQueryString(cblTopic, "TopicID", splitStr);
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
            }
            string[] strResults = new string[] { strResult, strQuery };
            return strResults;
        }        /// <summary>
        /// 初始化检索控件控件
        /// </summary>
        private void InitQueryControls(string cpName)
        {
            if (cpName == "NEULC")
            {
                divNEULC.Visible = true;
                divNEUAC.Visible = false;
                //话题
                string types = "Topic";
                DataSet dsTopic = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);
                CBLBindCorpusExt(dsTopic, cblTopic);

                //同时绑定WordList中的话题下拉列表框
                ddlTopics.DataSource = dsTopic.Tables[0].Copy();
                ddlTopics.DataTextField = "Title";
                ddlTopics.DataValueField = "ItemID";
                ddlTopics.DataBind();

                //文体
                types = "Genre";
                DataSet dsGenre = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);
                CBLBindCorpusExt(dsGenre, cblGenre);

                //年级
                types = "Level";
                DataSet dsLevel = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);
                CBLBindCorpusExt(dsLevel, cblLevel);
                divforCorpusResult.Visible = false;
                divNEULC.Visible = true;
            }
            else
            {
                divNEULC.Visible = false;
                divNEUAC.Visible = true;
            }
        }

        private void CBLBindCorpusExt(DataSet ds, CheckBoxList cblExt)
        {
            cblExt.Items.Clear();
            cblExt.DataSource = ds.Tables[0].Copy();
            cblExt.DataTextField = "Title";
            cblExt.DataValueField = "ItemID";
            cblExt.DataBind();
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

            foreach (ListItem itm in cblLevel.Items)
            {
                itm.Selected = false;
            }
            divforCorpusResult.Visible = false;
        }


        #endregion

        #endregion 2 Corpus：从整个语料数据库中按条件检索匹配的部分子语料库库

        #region 3 Concordance：检索语料库中与关键字的匹配的语料列表

        #region Concordance事件
        /// <summary>
        /// 提交检索Concordance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmitConcordance_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKeyConcordance.Value.Trim()))
            {
                lbErr.Text = "你还未输入要检索的关键词";
                txtKeyConcordance.Focus();
                return;
            }
            else
            {
                string keyConc = txtKeyConcordance.Value.Trim();
                string filterStr = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%'", keyConc);
                DataTable dtCorpus = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr).Tables[0];

                if (dtCorpus.Rows.Count > 0)
                {
                    int iCount = int.Parse(txtCDChars.Value.Trim());

                    int[] lAndr = GetLeftandRight(ddlCDCharacters, iCount);//iLeft & iRight
                    DataTable dtConcordance = Common.GetWordsFromCorpus(dtCorpus, keyConc, lAndr[0], lAndr[1]);
                    if (isShowTotalCount.Checked)
                    {
                        int rCount = dtConcordance.Rows.Count;
                        lbConcCount.Text = "Total Count in All Corpus is：" + rCount;
                    }
                    else
                    {
                        lbConcCount.Text = "";
                    }
                    GVBind(gvConcordance, dtConcordance);
                    ViewState["dtConcordance"] = dtConcordance;
                    divConcordanceQuery.Visible = false;
                    divConcordanceResult.Visible = true;
                    gvConcordance.Visible = true;
                    btnReConc.Visible = true;
                }
                else
                {
                    lbErr.Text = "语料库中没有与你检索的关键词相匹配的语料，请换个关键词再试！";
                    txtKeyConcordance.Focus();
                }

            }
        }

        private void GvConcComputed_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string strMatch = e.CommandArgument.ToString();
            DataTable dtConcordance = (DataTable)ViewState["dtConcordance"];
            DataView dv = dtConcordance.DefaultView;
            dv.RowFilter = string.Format("match = '{0}'", e.CommandArgument);
            DataTable dtFilter = dv.ToTable();
            ViewState["dtConcFilter"] = dtFilter;
            gvConcComputed.Visible = false;
            gvConcordance.Visible = true;
            btnCloseConc.Visible = true;
            GVBind(gvConcordance, dtFilter);
        }


        private void GvConcComputed_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvConcComputed.PageIndex = e.NewPageIndex;
            DataTable dtConcComputed = (DataTable)ViewState["dtConcComputed"];
            GVBind(gvConcComputed, dtConcComputed);
        }

        private void GvConcComputed_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //添加鼠标效果，当鼠标移动到行上时，变颜色
                e.Row.Attributes.Add("onmouseover", "currentcolor=this.style.backgroundColor;this.style.backgroundColor='#ccddff',this.style.cursor='pointer';");
                //当鼠标离开的时候 将背景颜色还原的以前的颜色
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=currentcolor,this.style.fontWeight='';");
            }
        }



        private void GvConcordance_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            throw new NotImplementedException();
        }



        private void GvConcordance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvConcordance.PageIndex = e.NewPageIndex;
            DataTable dtConcordance = (DataTable)ViewState["dtConcordance"];
            GVBind(gvConcordance, dtConcordance);
        }


        private void GvConcordance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //添加鼠标效果，当鼠标移动到行上时，变颜色
                e.Row.Attributes.Add("onmouseover", "currentcolor=this.style.backgroundColor;this.style.backgroundColor='#ccddff',this.style.cursor='pointer';");
                //当鼠标离开的时候 将背景颜色还原的以前的颜色
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=currentcolor,this.style.fontWeight='';");
            }
        }



        private void BtnReConc_Click(object sender, EventArgs e)
        {
            //btnCloseConc.Visible = true;
            divConcordanceQuery.Visible = true;
            divConcordanceResult.Visible = false;
            btnReConc.Visible = false;
        }


        private void BtnCloseConc_Click(object sender, EventArgs e)
        {
            //gvConcComputed.Visible = true;
            //gvConcordance.Visible = false;
            //btnCloseConc.Visible = false;
            //btnReConc.Visible = true;
        }

        #endregion Concordance事件

        #region Concordance方法

        private int[] GetLeftandRight(DropDownList ddl, int iCount)
        {
            int[] lAndr = new int[] { 0, 0 };
            string sme = ddl.SelectedValue;
            if (sme == "0")//在句首Start，则右边加单词
            {
                lAndr[1] = iCount;
            }
            else if (sme == "2")//End,则左边加单词
            {
                lAndr[0] = iCount;
            }
            else
            {
                lAndr[0] = iCount;
                lAndr[1] = iCount;
            }
            return lAndr;
        }

        #endregion Concordance方法

        #endregion 3 Concordance：检索语料库中与关键字的匹配的语料列表

        #region 4 Collocate：检索语料库中与关键字相关联的搭配

        #region Collocate事件

        /// <summary>
        /// 提交检索Collocate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmitCollocate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKeyCollocate.Value.Trim()))
            {
                lbErr.Text = "你还未输入要检索的关键词";
                txtKeyCollocate.Focus();
                return;
            }
            else
            {
                string keyColl = txtKeyCollocate.Value.Trim();
                string filterStr = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%'", keyColl);
                DataTable dtCorpus = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr).Tables[0];
                if (dtCorpus.Rows.Count > 0)
                {
                    int iCount = int.Parse(txtCCChars.Value.Trim());

                    int[] lAndr = GetLeftandRight(ddlCCCharacters, iCount);//iLeft & iRight
                    int mleft = int.Parse(txtcfLeft.Value.Trim());
                    int mright = int.Parse(txtcfRight.Value.Trim());
                    DataTable dtCollocate = Common.GetPhraseFromCorpus(keyColl, dtCorpus, mleft, mright, lAndr[0], lAndr[1]);
                    //查找所有和关键词相关的搭配的语料
                    DataTable dtCollComputed = Common.CaculatePhrase(dtCollocate);
                    ViewState["dtCollComputed"] = dtCollComputed;
                    GVBind(gvCollComputed, dtCollComputed);
                    ViewState["dtCollocate"] = dtCollocate;
                    divCollocateQuery.Visible = false;
                    divCollocateResult.Visible = true;
                    gvCollocate.Visible = false;
                    btnCloseColl.Visible = false;
                    gvCollComputed.Visible = true;
                }
                else
                {
                    lbErr.Text = "语料库中没有与你检索的关键词相匹配的语料，请换个关键词再试！";
                    txtKeyCollocate.Focus();
                }

            }
        }

        private void GvCollocate_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GvCollComputed_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //添加鼠标效果，当鼠标移动到行上时，变颜色
                e.Row.Attributes.Add("onmouseover", "currentcolor=this.style.backgroundColor;this.style.backgroundColor='#ccddff',this.style.cursor='pointer';");
                //当鼠标离开的时候 将背景颜色还原的以前的颜色
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=currentcolor,this.style.fontWeight='';");
            }
        }

        private void GvCollComputed_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCollComputed.PageIndex = e.NewPageIndex;
            DataTable dtCollComputed = (DataTable)ViewState["dtCollComputed"];
            GVBind(gvCollComputed, dtCollComputed);
        }

        private void GvCollComputed_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dtCollocate = (DataTable)ViewState["dtCollocate"];
            DataView dv = dtCollocate.DefaultView;
            dv.RowFilter = string.Format("match = '{0}'", e.CommandArgument);
            DataTable dtFilter = dv.ToTable();
            ViewState["dtCollFilter"] = dtFilter;
            gvCollocate.Visible = true;
            btnCloseColl.Visible = true;
            gvCollComputed.Visible = false;
            GVBind(gvCollocate, dtFilter);
        }


        private void GvCollocate_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCollocate.PageIndex = e.NewPageIndex;
            DataTable dtCollFilter = (DataTable)ViewState["dtCollFilter"];
            GVBind(gvConcordance, dtCollFilter);
        }



        private void GvCollocate_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //添加鼠标效果，当鼠标移动到行上时，变颜色
                e.Row.Attributes.Add("onmouseover", "currentcolor=this.style.backgroundColor;this.style.backgroundColor='#ccddff',this.style.cursor='pointer';");
                //当鼠标离开的时候 将背景颜色还原的以前的颜色
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=currentcolor,this.style.fontWeight='';");
            }
        }

        private void BtnReColl_Click(object sender, EventArgs e)
        {
            btnCloseColl.Visible = true;
            divCollocateQuery.Visible = true;
            divCollocateResult.Visible = false;
            btnReColl.Visible = false;
        }

        private void BtnCloseColl_Click(object sender, EventArgs e)
        {
            gvCollComputed.Visible = true;
            gvCollocate.Visible = false;
            btnCloseColl.Visible = false;
            btnReColl.Visible = true;
        }

        #endregion Collocate事件

        #region Collocate方法

        #endregion Collocate方法

        #endregion 4 Collocate：检索语料库中与关键字相关联的搭配

        #region 5 Cluster

        #region Cluster事件

        #endregion

        #region Cluster方法

        #endregion

        #endregion

        #region 6 WordList：将语料库或者用户输入文本按照词汇登记表标记文本中各个级别单词的分布

        #region WordList事件
        private void BtnLemmaAll_Click(object sender, EventArgs e)
        {
            string filterStr = ViewState["filterExp"].ToString();
            DataTable dtCorpusforWordList = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr).Tables[0];
            StringBuilder sb = new StringBuilder();

            foreach (DataRow dr in dtCorpusforWordList.Rows)
            {
                sb.AppendLine(dr["OriginalText"].ToString());
            }
            txtcontent.Value = sb.ToString();
        }


        private void GvCorpusforWordList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Label)e.Row.FindControl("lbText")) != null)
                {
                    Label lbText = (Label)(e.Row.FindControl("lbText"));
                    string strContext = lbText.Text;
                    string ignore = "[\r\n\t\"]";//需要替换的符号
                    strContext = Regex.Replace(strContext, ignore, "");
                    string js = string.Format("fillTextfromRow(\"{0}\");", strContext);
                    e.Row.Attributes.Add("onclick", js);

                    string[] paraWords = strContext.Split(' ');
                    int wordDisp = 10;
                    if (paraWords.Length < wordDisp)
                    {
                        wordDisp = paraWords.Length;

                    }
                    string temp = "";
                    for (int i = 0; i < wordDisp; i++)
                    {
                        temp += paraWords[i] + " ";
                    }
                    lbText.Text = temp.TrimEnd() + "...";
                    lbText.ToolTip = strContext;
                }

                //添加鼠标效果，当鼠标移动到行上时，变颜色
                e.Row.Attributes.Add("onmouseover", "currentcolor=this.style.backgroundColor;this.style.backgroundColor='#ccddff',this.style.cursor='pointer';");
                //当鼠标离开的时候 将背景颜色还原的以前的颜色
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=currentcolor,this.style.fontWeight='';");
            }
        }
        //分页事件
        private void GvCorpusforWordList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCorpusforWordList.PageIndex = e.NewPageIndex;
            Requery();
        }
        //重新绑定数据视图,分页时和加载时调用
        private void Requery()
        {
            string filterStr = ViewState["filterExp"].ToString();
            DataTable dt = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr).Tables[0];
            GVBind(gvCorpusforWordList, dt);
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
            divOutput.Visible = false;
            txtcontent.Value = "";//清空正文文本
            for (int i = 0; i < rbVBS.Items.Count; i++)//清除词表选择
            {
                if (rbVBS.Items[i].Selected)
                {
                    rbVBS.Items[i].Selected = false;
                }
            }
            txt_Title.Value = "";//清空正文标题
            divOutput.Visible = false;
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
            if (rbltxtFrom.SelectedValue == "0")//文本来自于用输入
            {
                divfromshuru.Visible = true;
                divTexts.Visible = true;
                divFromCorpus.Visible = false;
                txtKeyWordsforWordlist.Value = "";
                hdftxtFrom.Value = "0";
                txt_Title.Value = "";
            }
            else//文本来自于语料库
            {
                hdftxtFrom.Value = "1";
                divFromCorpus.Visible = true;
                divCorpusforWordList.Visible = true;
                Requery();
                divfromshuru.Visible = false;
                divTexts.Visible = true;
            }
            txtcontent.InnerText = "";
            divOutput.Visible = false;
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
            string rowFilter = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%'", keyWords);
            if (ViewState["filterExp"] != null)
            {
                rowFilter = string.Format("{0} And {1}", rowFilter, ViewState["filterExp"]);
            }
            DataTable dtCorpusforWordList = FSCDLL.DAL.Corpus.GetCorpusByFilterString(rowFilter).Tables[0];// dv.ToTable();
            if (dtCorpusforWordList.Rows.Count > 0)
            {
                divCorpusforWordList.Visible = true;
                GVBind(gvCorpusforWordList, dtCorpusforWordList);
            }
            else
            {
                lbErr.Text = string.Format("未检索到与关键词'{0}'相关的语料，请换个关键词重试！", keyWords);
                txtKeyWordsforWordlist.Focus();
            }
        }

        /// <summary>
        /// 从WordList界面返回，同时清空原有处理内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        private void BtnCloseLemma_Click(object sender, EventArgs e)
        {
            ReLemma(1);
        }

        /// <summary>
        /// 从WordList界面返回，不清空原有处理内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBackLemma_Click(object sender, EventArgs e)
        {
            ReLemma(0);
        }


        /// <summary>
        /// Lemma操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lemmanew_Click(object sender, EventArgs e)
        {
            #region 变量定义与表单校验
            string txtStr = "";//正文文本
            //检验文档正文是否输入完成
            if (string.IsNullOrEmpty(txtcontent.Value)) //处理的文本还未输入
            {
                PageAlert("你还未输入或导入需要处理的文本,请确认后再试！", this);
                txtcontent.Focus();
                return;
            }
            else
            {
                txtStr = txtcontent.Value;
                TextLemma(txtStr);
                string txtfrom = hdftxtFrom.Value;
                if (txtfrom == "0")//用户输入的文本处理
                {
                    #region  保存要处理的文本
                    //仅有处理非语料库内的文本时才做保存采集操作
                    int userId = FSCDLL.Common.Users.UserID;
                    DateTime dtNow = DateTime.Now;
                    string nowStr = string.Format("{0:yyyyMMddHHmmssffff}", dtNow);//时间格式字符串：年月日时分秒4位毫秒
                    string titleStr;//标题
                    string authorName = userId.ToString();
                    if (!string.IsNullOrEmpty(txt_Title.Value.Trim()) && txt_Author.Value.Trim() != "Type the Author's Name")
                    {
                        authorName = txt_Title.Value.Trim();
                    }
                    if (string.IsNullOrEmpty(txt_Title.Value.Trim()) || txt_Title.Value.Trim() == "Type the title")//标题为空或者为文本框提示值,即未输入标题
                    {

                        titleStr = string.Format("{0}({1})", nowStr, authorName);
                    }
                    else
                    {
                        titleStr = string.Format("{0}({1})", txt_Title.Value.Trim(), authorName);//标题
                    }

                    DataTable dtCorpus = FSCDLL.DAL.Corpus.GetCorpus().Tables[0];
                    DataRow drCorpus = dtCorpus.NewRow();
                    drCorpus["Title"] = titleStr;
                    drCorpus["OriginalText"] = txtcontent.Value;
                    drCorpus["Created"] = dtNow;
                    drCorpus["Author"] = userId;
                    drCorpus["TopicID"] = splitStr + ddlTopics.SelectedValue + splitStr;
                    drCorpus["Flag"] = 3;
                    FSCDLL.DAL.Corpus.InsertCorpus(null, drCorpus);
                    #endregion  保存要处理的文本

                    divfromshuru.Visible = false;//输入表单隐藏
                }
                else
                {
                    divFromCorpus.Visible = false;
                }
                divTexts.Visible = false;//文本框隐藏
                divOutput.Visible = true;//输出结果显示
            }
            #endregion 变量定义与表单校验

        }
        #endregion

        #region WordList方法

        /// <summary>
        /// 返回WordList处理前界面
        /// </summary>
        /// <param name="isClear">是否清空</param>
        private void ReLemma(int isClear)
        {
            string txtFrom = hdftxtFrom.Value;
            if (txtFrom == "0")//本次处理的是用户输入的文本，则仍返回用户输入界面
            {
                divfromshuru.Visible = true;//输入表单显示
                divTexts.Visible = true;//主文本框显示
                divFromCorpus.Visible = false;//从语料库选择文本界面隐藏
                if (isClear == 1)
                {
                    txt_Author.Value = "";
                    txt_Title.Value = "";
                    txtcontent.Value = "";
                    ddlTopics.SelectedIndex = 0;
                    rbVBS.ClearSelection();
                }
            }
            else
            {
                divFromCorpus.Visible = true;
                divTexts.Visible = true;
                if (isClear == 1)
                {
                    txtKeyWordsforWordlist.Value = "";
                    rbVBS.ClearSelection();
                }
            }
            divContext.InnerHtml = "等待处理结果中...";
            dlChart.InnerHtml = "等待处理结果中...";
            divOutput.Visible = false;//输出结果隐藏
        }

        private void AbstinencyRadioListOption(ref RadioButtonList rbtList)
        {
            for (int i = 0; i < rbtList.Items.Count; i++)
            {
                if (rbtList.Items[i].Value == "1")
                {
                    rbtList.Items[i].Enabled = false;
                }
            }

        }

        private void TextLemma(string txtStr)
        {
            #region 0 先过滤掉网址等干扰文本
            string regEx = @"((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";
            txtStr = Regex.Replace(txtStr, regEx, ";");//正则表达式排除文中的网址
            #endregion 0 先过滤掉网址等干扰文本

            #region 1 过滤文本判断是否包含有英文单词
            string dbPath = GetDbPath();
            string ignoreWordsFile = dbPath + "words/ignoreWords.txt";
            string ordinalWordsFile = dbPath + "words/OrdinalWords.txt";
            string symbolFile = dbPath + "words/symbol.txt";

            List<List<string>> txtlist = TextInput.ArticleToList(txtStr, ignoreWordsFile, ordinalWordsFile, symbolFile);//文本转化为字符串数组,将需要处理的单词存到数组中
            if (txtlist.Count == 0)//文本中不包含有英文单词
            {
                PageAlert("文本中不包含需要处理的英文单词！", this);
                txtcontent.Focus();
            }
            #endregion 1 过滤文本判断是否包含有英文单词
            else
            {
                #region 2 参照词库选择
                int maxIndex = 4;
                #endregion 2 参照词库选择

                #region 3 单词还原
                string fileName = dbPath + "words/AllWords.txt";//包含原型与变型以及对应等级的词汇表
                Dictionary<int, object> allwordsList = WordBLL.SearchWordsWithTxt(txtlist, fileName, 0);//对词汇列表进行比对还原和级别确认，输出三个数据集：1、文本词汇对应级别，2、超纲词汇对应词频，3、处理过的单词原型对应级别
                #endregion 3 单词还原

                #region 4 WordList和结果输出
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
                }
                #endregion 4 WordList和结果输出
            }

        }

        private string GetDbPath()
        {
            string txtPath = Server.MapPath("");
            txtPath = txtPath.Substring(0, txtPath.IndexOf("\\layouts", StringComparison.Ordinal)) + @"\layouts\db\";
            return txtPath;
        }




        #endregion WordList方法

        #endregion 6 WordList：将语料库或者用户输入文本按照词汇登记表标记文本中各个级别单词的分布

        #region 7 Compare:比较两到三个关键词的使用频率
        #region Compare事件
        /// <summary>
        /// 比较输入的多个关键词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCompared_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtfreqField1.Value.Trim()) || string.IsNullOrEmpty(txtfreqField2.Value.Trim()))
            {
                lbErr.Text = "请至少在前两个文本框中输入比较关键词!";
            }
            else
            {
                string cTitle = "";
                string filterStr = ViewState["filterExp"].ToString();
                if (rblforCompare.SelectedValue == "All" || string.IsNullOrEmpty(filterStr))
                {
                    string corpus = GetCorpusByUrl();
                    filterStr = "Source = " + corpus;
                }
                DataTable dtComputed = InitComputedTable();
                string keyWord = txtfreqField1.Value.Trim();
                cTitle += string.Format("\"{0}\"", keyWord);
                string filterStr1 = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%' And {1}", keyWord, filterStr);
                DataTable dt1 = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr1).Tables[0];
                GetComputedDate(ref dtComputed, dt1, keyWord);

                keyWord = txtfreqField2.Value.Trim();
                cTitle += string.Format(",\"{0}\"", keyWord);
                string filterStr2 = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%' And {1}", keyWord, filterStr);
                DataTable dt2 = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr2).Tables[0];
                GetComputedDate(ref dtComputed, dt2, keyWord);

                if (string.IsNullOrEmpty(txtfreqField3.Value.Trim()))
                {
                    keyWord = txtfreqField2.Value.Trim();
                    cTitle += string.Format(",\"{0}\"", keyWord);
                    string filterStr3 = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%' And {1}", keyWord, filterStr);
                    DataTable dt3 = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr3).Tables[0];
                    GetComputedDate(ref dtComputed, dt3, keyWord);
                }
                ChartBind(dtComputed, cTitle + "使用次数与分布语篇数", SeriesChartType.RangeBar);
            }

        }



        private void GetComputedDate(ref DataTable dtComputed, DataTable dtSource, string wd)
        {
            DataRow dr = dtComputed.NewRow();
            dr[0] = wd;
            dr[1] = dtSource.Rows.Count;
            dr[2] = dtSource.DefaultView.ToTable(true, "CorpusID").Rows.Count;
            dtComputed.Rows.Add(dr);

        }


        #endregion Compare事件
        #region Compare方法
        /// <summary>
        /// Chart初始化
        /// </summary>
        /// <param name="mychart">chart控件Id</param>
        /// <param name="chartTitle">chart标题</param>
        /// <param name="chartType">chart图像类型</param>
        private void InitChart(Chart mychart, string chartTitle, SeriesChartType chartType, DataTable dtComputed)
        {
            //图表标题
            mychart.Titles.Clear();
            mychart.Titles.Add(chartTitle);
            mychart.Titles[0].Text = chartTitle;
            mychart.Titles[0].ForeColor = Color.Red;
            //清除默认的series
            mychart.Series.Clear();
            for (int i = 0; i < dtComputed.Rows.Count; i++)
            {
                DataRow dr = dtComputed.Rows[i];
                Series ss = new Series()
                {
                    ChartType = chartType,
                    MarkerBorderWidth = 2,
                    MarkerSize = 4,
                    Name = dr[0].ToString(),
                    MarkerStyle = MarkerStyle.Circle,
                    ToolTip = dr[0].ToString() + " #VAL \r\n #AXISLABEL",
                    XValueType = ChartValueType.String,
                    Label = "#VAL"
                };

                if (ss.ChartType == SeriesChartType.Pie)
                {
                    ss.LabelToolTip = "#PERCENT{P}";
                }
                ss.ShadowColor = Color.Black;
                ss.BorderColor = Color.Brown;
                ss.LegendText = ss.Name;
                mychart.Series.Add(ss);
            }

        }


        //创建表
        private DataTable InitComputedTable()
        {
            DataTable dt = new DataTable();
            //添加列
            dt.Columns.Add("Words");
            dt.Columns.Add("WCount");
            dt.Columns.Add("WFreq");
            return dt;
        }

        private void ChartBind(DataTable dtComputed, string chartTitle, SeriesChartType cType)
        {
            InitChart(chartForCompare, chartTitle, cType, dtComputed);


            //Color[] bgcolor = new Color[3] { Color.Red, Color.Blue, Color.Green };
            //Color[] lnColor = new Color[3] { Color.FromArgb(50, 255, 0, 0), Color.FromArgb(50, 0, 0, 255), Color.FromArgb(50, 0, 255, 0) };

            //数据绑定
            for (int i = 0; i < dtComputed.Columns.Count - 1; i++)//在前台设置好Series的个数与yFields个数相同，并设定好每个Series的样式
            {
                string xField = dtComputed.Columns[0].ColumnName;
                string yField = dtComputed.Columns[i + 1].ColumnName;
                chartForCompare.Series[i].Points.DataBind(dtComputed.DefaultView, xField, yField, string.Format("LegendText={0},YValues={1},ToolTip={0}", xField, yField));
                chartForCompare.Series[i].ToolTip = chartForCompare.Series[i].Name + " #VAL \r\n #AXISLABEL";
                chartForCompare.Series[i].ChartType = cType;
            }


            //背景色设置
            chartForCompare.ChartAreas[0].ShadowColor = Color.Transparent;
            chartForCompare.ChartAreas[0].BackColor = Color.FromArgb(209, 237, 254);//该处设置为了由天蓝到白色的逐渐变化
            chartForCompare.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
            chartForCompare.ChartAreas[0].BackSecondaryColor = Color.White;

            //中间X,Y线条的颜色设置
            chartForCompare.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartForCompare.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);

            //3D设置
            chartForCompare.ChartAreas[0].Area3DStyle.Enable3D = true;


            chartForCompare.DataBind();
        }

        #endregion
        #endregion
    }
}
