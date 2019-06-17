using FSCDLL.DAL;
using lemmatizerDLL;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{
    public partial class neulc : LayoutsPageBase
    {
        #region 0 公用
        /// <summary>
        /// Url中传递的CorpusName的参数值
        /// </summary>
        private string CorpusName => Request.QueryString["cp"] == null ? "NEULC" : Request.QueryString["cp"].ToUpper();
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
            muNeulc.Attributes.Add("onclick", "javascript:shield();");
            //Concordance提交按钮点击事件
            btnSubmitforCorpus.Click += BtnSubmitforCorpus_Click;
            btnSubmitforCorpus.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            //WordList控件事件绑定
            btnBacktoQuery.Click += BtnBacktoQuery_Click;
            btnBacktoQuery.Attributes.Add("onclick", "javascript:shield();");
            btnSubmitForLemma.Click += BtnSubmitForLemma_Click;//WordList提交按钮点击事件
            btnSubmitForLemma.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            btnBackLemma.Click += BtnBackLemma_Click;
            btnBackLemma.Attributes.Add("onclick", "javascript:shield();");

            rbltxtFrom.SelectedIndexChanged += RbltxtFrom_SelectedIndexChanged;
            btnQueryforWordlist.Click += BtnQueryforWordlist_Click;
            btnQueryforWordlist.Attributes.Add("onclick", "javascript:shield();");
            gvCorpusforWordList.RowDataBound += GvCorpusforWordList_RowDataBound;
            gvCorpusforWordList.PageIndexChanging += GvCorpusforWordList_PageIndexChanging;
            btnLemmaAll.Click += BtnLemmaAll_Click;
            btnLemmaAll.Attributes.Add("onclick", "javascript:shield();");
            clearBtn.Click += clearBtn_OnClick;
            clearBtn.Attributes.Add("onclick", "javascript:shield();");

            //Concordance 内的控件事件绑定
            btnSubmitConcordance.Click += BtnSubmitConcordance_Click;
            btnSubmitConcordance.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            btnReConc.Click += BtnReConc_Click;
            btnReConc.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            gvConcordance.RowDataBound += GvConcordance_RowDataBound;
            gvConcordance.PageIndexChanging += GvConcordance_PageIndexChanging;
            gvConcordance.RowCommand += GvConcordance_RowCommand;
            btnViewConc.Click += BtnViewConc_Click;
            btnViewConc.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            //Collocate 内的控件事件绑定
            btnSubmitCollocate.Click += BtnSubmitCollocate_Click;
            btnSubmitCollocate.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            btnReColl.Click += BtnReColl_Click;
            btnReColl.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示

            gvCollocate.RowDataBound += GvCollocate_RowDataBound;
            gvCollocate.PageIndexChanging += GvCollocate_PageIndexChanging;
            gvCollocate.RowCommand += GvCollocate_RowCommand;

            gvCollComputed.RowCommand += GvCollComputed_RowCommand;
            gvCollComputed.PageIndexChanging += GvCollComputed_PageIndexChanging;
            gvCollComputed.RowDataBound += GvCollComputed_RowDataBound;
            btnCloseColl.Click += BtnCloseColl_Click;
            btnCloseColl.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            btnViewColl.Click += BtnViewColl_Click;
            btnViewColl.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            //Compare提交按钮点击事件
            btnCompared.Click += BtnCompared_Click;
            btnCompared.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            btnBackToCompare.Click += BtnBackToCompare_Click;
            btnBackToCompare.Attributes.Add("onclick", "javascript:shield();");//为按钮点击事件的处理过程增加提示
            //词汇表选择
            rbVBS.SelectedIndexChanged += RbVBS_SelectedIndexChanged;


            //页面加载
            if (!IsPostBack)
            {
                InitCorpus();

                ShowCorpusByName();
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
            }

        }


        private string ContactContextinTable(DataTable dtSource, string keyCol)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                DataRow dr = dtSource.Rows[i];
                string strContext = SystemDataExtension.GetString(dr, keyCol);
                if (strContext != string.Empty)
                {
                    sb.AppendLine(strContext);
                }
            }
            return sb.ToString();
        }

        private void RbVBS_SelectedIndexChanged(object sender, EventArgs e)
        {
            string libName = rbVBS.SelectedValue;
            string txtToLemma = ViewState["LemmaContext"].ToString();
            string lastViewIndex = hdfvwIndex.Value;
            ShowContext(null, txtToLemma, libName, lastViewIndex);
        }

        #endregion
        #region 公用方法

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


        public StringBuilder GetHighlightContext(IEnumerable<List<string>> showWordsList, string keyWords)
        {
            StringBuilder sb = new StringBuilder();
            string[] strENLevels = new string[7] { "UN", "C1", "C2", "A1", "A2", "B1", "B2" };//级别英文简称，主要用于样式表对应

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
                string sp = string.Format("<span class='{0}'> {1}</span>", className, sword);
                if (keyWords != "" && keyWords == sword)
                {
                    sp = string.Format("<strong>{0}</strong>", sp);
                }
                sb.AppendFormat(sp);
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

            lbErr.Text = "";
            ViewState["filterExp"] = null;
            switch (muNeulc.SelectedValue)
            {
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
                    InitCorpus();
                    break;
            }
            Titlelb.Text = "> " + muNeulc.SelectedItem.Text;

        }


        #endregion

        #region 顶部工具栏方法

        /// <summary>
        /// 初始化菜单项，根据URL中的参数，改变菜单的显示
        /// </summary>
        private void InitCorpus()
        {
            lbCorpus.Text = CorpusName;
            if (CorpusName == "NEULC")
            {
                divNEULC.Visible = true;
                divNEUAC.Visible = false;
            }
            else
            {
                divNEULC.Visible = false;
                divNEUAC.Visible = true;
            }
            divforCorpusResult.Visible = false;
        }

        #endregion 顶部工具栏方法

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
            ShowCorpusByName();
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
                rblforCompare.Items[1].Enabled = false;
                return;
            }
            else
            {
                lbErr.Text = "";
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
                //divCPTips.InnerHtml = string.Format("<span class='gvtips'>{0}</span>", newQueryStr);
                QueryCorpus(newQueryStr);//检索语料库
                divforCorpusResult.Visible = true;
                divNEULC.Visible = false;
                rblforCompare.Items[1].Enabled = true;//经过检索后，Compare中才可以使用该检索结果做为比较用语料
            }

        }


        #endregion Corpus事件

        #region Corpus方法        /*特别注释：NEUAC的三个筛选条件：Year、Major、Journal与NEULC的三个筛选条件：Level、Topic、Genre 一一对应*/
        /// <summary>
        /// 根据Url传递的Corpus参数名称来决定使用哪一个Corpus
        /// </summary>
        private void ShowCorpusByName()
        {
            if (CorpusName == "NEULC")
            {
                divNEULC.Visible = true;
                divNEUAC.Visible = false;
                ibtnUpload.PostBackUrl = "admin.aspx?Source=LC";
            }
            else
            {
                divNEUAC.Visible = true;
                divNEULC.Visible = false;
                ibtnUpload.PostBackUrl = "admin.aspx?Source=AC";
            }
            hlinkPageTitle1.NavigateUrl = "neulc.aspx?cp=" + CorpusName;
            hlinkPageTitle1.Text = CorpusName;
        }
        /// <summary>
        /// 根据筛选条件构造语料库的汇总分析数据表DataTable
        /// </summary>
        /// <param name="dtCorpus">语料库源数据表</param>
        /// <param name="fkid">外键ID</param>
        /// <param name="cbl">筛选条件项</param>
        /// <param name="listFKs">筛选条件汇总关联项</param>
        /// <returns>汇总表</returns>
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
            listFKs.Remove(fkid);//删除本次针对性的元素
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
            listFKs.Add(fkid);//将数组删除的元素还原
            return dtSummary;
        }


        /// <summary>
        /// 获取文本的统计信息
        /// </summary>
        /// <param name="paraContext">文本内容</param>
        private Dictionary<int, string> GetTextRange(string paraContext)
        {
            Dictionary<int, string> dictRange = new Dictionary<int, string>();
            List<string> listWords = new List<string>();
            List<int> listParaLength = new List<int>();
            Dictionary<int, List<string>> dictPara = Common.ParseSentences(paraContext);//对本类别下的语篇进行句子拆分，生成句子序列和句子对应的单词词组
            foreach (int key in dictPara.Keys)
            {
                listParaLength.Add(dictPara[key].Count);
                foreach (string wd in dictPara[key])
                {
                    listWords.Add(wd);//获取单词长度
                }
            }
            double avg = listParaLength.Average();
            dictRange.Add(5, string.Format("<th>mean in words</th><td>{0}</td>", avg.ToString("0.00")));//平均句长 mean in words
            double sum = listParaLength.Sum(d => Math.Pow(d - avg, 2));//计算各数值与平均数的差值的平方，然后求和
            //除以数量，然后开方
            double standard = Math.Sqrt(sum / listParaLength.Count);
            dictRange.Add(6, string.Format("<th>standard deviation</th><td>{0}</td>", standard.ToString("0.00")));//句长标准差


            int tokens = listWords.Count;
            List<int> listWordLength = new List<int>();
            foreach (string wd in listWords)
            {
                listWordLength.Add(wd.Length);
            }
            avg = listWordLength.Average();
            dictRange.Add(3, string.Format("<th>mean word length (in characters)</th><td>{0}</td>", avg.ToString("0.00")));//平均词长
            sum = listWordLength.Sum(d => Math.Pow(d - avg, 2));
            standard = Math.Sqrt(sum / tokens);
            dictRange.Add(4, string.Format("<th>mean word length standard deviation</th><td>{0}</td>", standard.ToString("0.00")));//词长标准差


            dictRange.Add(1, string.Format("<th>tokens</th><td>{0}</td>", tokens));//单词总数 tokens

            listWords = listWords.Distinct().ToList();
            int types = listWords.Count;
            dictRange.Add(0, string.Format("<th>types</th><td>{0}</td>", types));//去重复后单词数 types
            double ttr = Convert.ToDouble(types) / Convert.ToDouble(tokens);
            dictRange.Add(2, string.Format("<th>TTR</th><td>{0}</td>", ttr.ToString("0.00%")));//TTR

            return dictRange;
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
        private void QueryCorpus(string filterExpression)
        {
            DataSet dsResult = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterExpression);
            DataTable dtResult = dsResult.Tables[0].Copy();
            List<string> listFKs = new List<string> { "LevelID", "TopicID", "GenreID" };
            BuildTable(dtResult, cblLevel, "LevelID", tbforLevel, listFKs);

            BuildTable(dtResult, cblTopic, "TopicID", tbforTopic, listFKs);

            BuildTable(dtResult, cblGenre, "GenreID", tbforGenre, listFKs);
        }        /// <summary>
        /// 构造检索字符串，用于检索大库
        /// </summary>
        /// <returns>检索状态和检索结果字符串形成的数组</returns>
        private string[] GetSelectQuery()
        {
            string splitStr = "";
            if (CorpusName == "NEULC")
            {
                splitStr = ";";
            }
            string strQuery = "";
            string strResult = "";
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


            if (strQuery == "")
            {
                strQuery = string.Format("Source = '{0}'", CorpusName);
            }
            else
            {
                if (strQuery.IndexOf("and") > 0)
                {
                    strQuery = string.Format("{0} and (Source = '{1}')", strQuery, CorpusName);
                }
                else
                {
                    strQuery = string.Format("({0}) and (Source = '{1}')", strQuery, CorpusName);
                }
            }

            string[] strResults = new string[] { strResult, strQuery };
            return strResults;
        }        /// <summary>
        /// 初始化检索控件控件
        /// </summary>
        private void InitQueryControls()
        {
            if (CorpusName == "NEULC")
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

            }
            else
            {
                divNEULC.Visible = false;
                divNEUAC.Visible = true;
                //年份
                BindACYear();

                //专业
                DataSet dsMajor = FSCDLL.DAL.Corpus.GetCopusExtendByTypes("Major");
                CBLBindCorpusExt(dsMajor, cblMajor);
                //期刊与专业联动，需要在专业选定时才绑定
            }

            divforCorpusResult.Visible = false;
        }


        /// <summary>
        /// 绑定NEUAC中年份多选控件
        /// </summary>
        private void BindACYear()
        {
            DataSet dsYears = FSCDLL.DAL.Corpus.GetCorpusYearsOfAC();
            DataTable dtYears = dsYears.Tables[0];
            cblYear.Items.Clear();
            cblYear.DataSource = dtYears;
            cblYear.DataTextField = "YEAR";
            cblYear.DataValueField = "YEAR";
            cblYear.DataBind();
        }


        /// <summary>
        /// CheckBoxList控件绑定数据源
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="cblExt">CheckBoxList控件ID</param>
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
            if (CorpusName == "NEULC")
            {
                divNEULC.Visible = true;

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
            }
            else
            {
                divNEUAC.Visible = true;
                foreach (ListItem itm in cblYear.Items)
                {
                    itm.Selected = false;
                }

                foreach (ListItem itm in cblMajor.Items)
                {
                    itm.Selected = false;
                }

                foreach (ListItem itm in cblJournal.Items)
                {
                    itm.Selected = false;
                }
            }
            divforCorpusResult.Visible = false;
        }


        #endregion Corpus方法

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
                lbErr.Text = "";
                string keyConc = txtKeyConcordance.Value.Trim();
                ViewState["KeyWords"] = keyConc;
                string filterStr = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%' and Source = '{1}'", keyConc, CorpusName);
                //if (ViewState["filterExp"] != null)
                //{
                //    filterStr += " and " + ViewState["filterExp"].ToString();
                //}
                DataTable dtCorpus = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr).Tables[0];
                dtCorpus.TableName = "Table-Corpus";
                if (dtCorpus.Rows.Count > 0)
                {
                    int iCount = int.Parse(txtCDChars.Value.Trim());

                    int[] lAndr = GetLeftandRight(ddlMatchPos, iCount);//iLeft & iRight
                    DataTable dtConcordance = Common.GetWordsFromCorpus(dtCorpus, keyConc, lAndr[0], lAndr[1]);
                    dtConcordance.TableName = "Table-Concordance";
                    int rCount = dtConcordance.Rows.Count;
                    int dispCount = rCount;
                    string showLimit = txtLimit.Value;
                    if (showLimit != "0")//指定输出的行数
                    {
                        dispCount = int.Parse(showLimit);
                        dtConcordance = dtConcordance.AsEnumerable().Take(dispCount).CopyToDataTable<DataRow>();
                        showLimit = string.Format("You choose to display <strong>{0}</strong>", showLimit);
                    }
                    else
                    {
                        showLimit = "You choose to display <strong>All</strong>";

                    }
                    string pageSize = txtRpp.Value;
                    gvConcordance.PageSize = int.Parse(pageSize);
                    showLimit = string.Format("{0} matches with <strong>{1}</strong> per page", showLimit, pageSize);

                    spConcCount.InnerHtml = string.Format("Total number of matches with <strong>\"{0}\"</strong> is <strong>{1}</strong> ; {2}", keyConc, rCount, showLimit);
                    GVBind(gvConcordance, dtConcordance);

                    ViewState["dtConcordance"] = dtConcordance;
                    divConcordanceQuery.Visible = false;
                    divConcordanceResult.Visible = true;
                    divConcTips.Visible = true;
                    if (dispCount > 100)
                    {
                        spConcTips.InnerHtml = string.Format("Click on the <strong>\"Title\"</strong> in each row of the list to view the corpus context;The \"View All\" Button is disabled because Total number of matches with <strong>\"{0}\"</strong> is larger than 100.", keyConc);
                        btnViewConc.Enabled = false;
                    }
                    else
                    {
                        spConcTips.InnerHtml = "Click on the <strong>\"Title\"</strong> in each row of the list to view the corpus context";
                        btnViewConc.Enabled = true;
                    }
                }
                else
                {
                    lbErr.Text = "语料库中没有与你检索的关键词相匹配的语料，请换个关键词再试！";
                    txtKeyConcordance.Focus();
                    divConcTips.Visible = false;
                }

            }
        }

        /// <summary>
        /// 浏览所有的检索到的Concordance匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnViewConc_Click(object sender, EventArgs e)
        {
            DataTable dtConcordance = (DataTable)ViewState["dtConcordance"];
            string strContext = ContactContextinTable(dtConcordance, "OriginalText");
            ShowContext(null, strContext, "CECR", "1");

        }

        private void GvConcordance_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Page") return;
            string cpId = e.CommandArgument.ToString();
            DataTable dtCorpusExtend;
            if (ViewState["dtCorpusExtend"] == null)
            {
                dtCorpusExtend = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(null).Tables[0];
                ViewState["dtCorpusExtend"] = dtCorpusExtend.Copy();
            }
            else
            {
                dtCorpusExtend = (ViewState["dtCorpusExtend"] as DataTable).Copy();
            }
            DataTable dtSource = Common.GetCorpusByID(long.Parse(cpId), CorpusName, dtCorpusExtend);
            if (dtSource != null)
            {
                DataRow drSource = dtSource.Rows[0];
                string kWords = ViewState["KeyWords"].ToString();
                ShowContext(drSource, "", "CECR", "1");
            }
            else
            {
                mvNeulc.ActiveViewIndex = 1;
                hdfvwIndex.Value = "0";
                lbErr.Text = "语料库中不存在该条语料或已删除！";
            }

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
                if (e.Row.FindControl("lnkBtn") != null)
                {
                    LinkButton lnkBtn = (LinkButton)e.Row.FindControl("lnkBtn");
                    lnkBtn.Attributes.Add("onclick", "javascript:shield();");
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                if (ViewState["HideCol"] != null)
                {
                    e.Row.Cells[int.Parse(ViewState["HideCol"].ToString())].Visible = false; //隐藏ID列,可以取得该隐藏列的信息
                }
            }
        }

        private void BtnReConc_Click(object sender, EventArgs e)
        {
            divConcordanceQuery.Visible = true;
            divConcordanceResult.Visible = false;
        }

        #endregion Concordance事件

        #region Concordance方法

        /// <summary>
        /// 获取语料信息
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="dr">要处理的正文</param>
        /// <param name="kWords">关键词</param>
        /// <param name="libName">词汇表名称</param>
        private StringBuilder GetContextInfo(DataRow dr, string strContext, string kWords, string libName)
        {
            StringBuilder sb = new StringBuilder();
            string txtStr = strContext;
            sb.AppendLine("<fieldset id='fdsCInfo'><legend><span style='font-size:14px;font-weight:bold;'>Context Infomation</span></legend>");
            sb.AppendLine("<table class='infoTable'>");
            if (dr != null)
            {
                txtStr = SystemDataExtension.GetString(dr, "OriginalText");
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<th>Title</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Title")));
                sb.AppendLine(string.Format("<th>Created</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Created")));
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<th>Source</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Source")));

                if (CorpusName == "NEULC")
                {
                    sb.AppendLine(string.Format("<th>Topic</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Topic")));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr>");
                    sb.AppendLine(string.Format("<th>Genre</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Genre")));
                    sb.AppendLine(string.Format("<th>Level</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Level")));
                }
                else
                {
                    sb.AppendLine(string.Format("<th>Major</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Major")));
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr>");
                    sb.AppendLine(string.Format("<th>Journal</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Journal")));
                    sb.AppendLine(string.Format("<th>Year</th><td>{0}</td>", SystemDataExtension.GetString(dr, "Year")));
                }
                sb.AppendLine("</tr>");
            }
            if (kWords != "")
            {
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<th>KeyWord</th><td><strong>{0}</strong></td>", kWords));
                sb.AppendLine(string.Format("<th>The number of KeyWord in this corpus</th><td>{0}</td>", Regex.Matches(txtStr, kWords).Count));
                sb.AppendLine("</tr>");
            }

            Dictionary<int, string> dictRange = GetTextRange(txtStr);
            var dictSort = from objDic in dictRange orderby objDic.Key ascending select objDic;
            sb.AppendLine("<tr>");
            foreach (KeyValuePair<int, string> kvp in dictSort)
            {
                if (kvp.Key % 2 == 0)
                {
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr>");
                }
                sb.AppendLine(kvp.Value);
            }
            sb.AppendLine(string.Format("<th>vocabulary</th><td>{0}</td>", libName));
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</fieldset>");
            return sb;
        }

        /// <summary>
        /// 从单词位置标记下拉框和单词匹配数，获取匹配项的左右各要匹配的单词数
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="iCount"></param>
        /// <returns></returns>
        private int[] GetLeftandRight(DropDownList ddl, int iCount)
        {
            int[] lAndr = new int[] { 0, 0 };
            string sme = ddl.SelectedValue;
            if (sme == "0")//在句首Start，则右边加单词
            {
                lAndr[1] = iCount;
                ViewState["HideCol"] = 1;
            }
            else if (sme == "2")//End,则左边加单词
            {
                lAndr[0] = iCount;
                ViewState["HideCol"] = 3;
            }
            else
            {
                lAndr[0] = iCount;
                lAndr[1] = iCount;
                ViewState["HideCol"] = null;
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
                lbErr.Text = "";
                string keyColl = txtKeyCollocate.Value.Trim();
                ViewState["KeyWords"] = keyColl;
                string filterStr = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%' and Source ='{1}'", keyColl, CorpusName);
                DataTable dtCorpus = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr).Tables[0];
                if (dtCorpus.Rows.Count > 0)
                {
                    int iCount = int.Parse(txtCCChars.Value.Trim());

                    int[] lAndr = GetLeftandRight(ddlCollocatesPos, iCount);//iLeft & iRight
                    int mleft = int.Parse(txtcfLeft.Value.Trim());
                    int mright = int.Parse(txtcfRight.Value.Trim());
                    DataTable dtCollocate = Common.GetPhraseFromCorpus(keyColl, dtCorpus, mleft, mright, lAndr[0], lAndr[1]);
                    dtCollocate.TableName = "Table-Collocate";
                    //查找所有和关键词相关的搭配的语料
                    if (dtCollocate.Rows.Count > 0)
                    {
                        string matchKey = GetTags(mleft, "*") + keyColl + GetTags(mright, "*");
                        DataTable dtCollComputed = Common.CaculatePhrase(dtCollocate);
                        dtCollComputed.DefaultView.Sort = "totalTimes desc";
                        dtCollComputed.TableName = "Table-CollComputed";
                        ViewState["dtCollComputed"] = dtCollComputed;
                        spCoLLComputedCount.InnerHtml = string.Format("Total number of matches with <strong>\"{0}\"</strong> is <strong>{1}</strong>", matchKey, dtCollComputed.Rows.Count);

                        GVBind(gvCollComputed, dtCollComputed);
                        ViewState["dtCollocate"] = dtCollocate;
                        divCollocateQuery.Visible = false;
                        divCollocateResult.Visible = true;
                        divCollComputed.Visible = true;
                        divCollView.Visible = false;
                        divCollTips.Visible = true;
                    }
                    else
                    {
                        lbErr.Text = string.Format("语料库中没有与你检索的关键词 \"{0}\" 相匹配的语料，请换个关键词再试！", keyColl);
                        txtKeyCollocate.Focus();
                        divCollTips.Visible = false;
                    }
                }
                else
                {
                    lbErr.Text = string.Format("语料库中没有与你检索的关键词 \"{0}\" 相匹配的语料，请换个关键词再试！", keyColl);
                    txtKeyCollocate.Focus();
                }

            }
        }

        private string GetTags(int tagCount, string strTag)
        {
            string stars = " ";
            for (int i = 0; i < tagCount; i++)
            {
                stars += strTag + " ";
            }
            return stars;

        }

        /// <summary>
        /// 浏览所有的检索到的Collocate匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnViewColl_Click(object sender, EventArgs e)
        {
            DataTable dtCollFilter = (DataTable)ViewState["dtCollFilter"];
            string strContext = ContactContextinTable(dtCollFilter, "OriginalText");
            ShowContext(null, strContext, "CECR", "2");
        }

        private void GvCollocate_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Page") return;
            string cpId = e.CommandArgument.ToString();

            DataTable dtCorpusExtend;
            if (ViewState["dtCorpusExtend"] == null)
            {
                dtCorpusExtend = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(null).Tables[0];
                ViewState["dtCorpusExtend"] = dtCorpusExtend.Copy();
            }
            else
            {
                dtCorpusExtend = (ViewState["dtCorpusExtend"] as DataTable).Copy();
            }
            DataTable dt = Common.GetCorpusByID(long.Parse(cpId), CorpusName, dtCorpusExtend);
            if (dt != null)
            {
                DataRow dr = dt.Rows[0];
                string txtStr = SystemDataExtension.GetString(dr, "OriginalText");
                ViewState["LemmaContext"] = txtStr;
                ShowContext(dr, "", "CECR", "2");//缺省第一次采用CECR词汇表计算
            }
            else
            {
                mvNeulc.ActiveViewIndex = 2;
                hdfvwIndex.Value = "0";
                lbErr.Text = "语料库中不存在该条语料或已删除！";
            }
        }

        private void GvCollComputed_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //添加鼠标效果，当鼠标移动到行上时，变颜色
                e.Row.Attributes.Add("onmouseover", "currentcolor=this.style.backgroundColor;this.style.backgroundColor='#ccddff',this.style.cursor='pointer';");
                //当鼠标离开的时候 将背景颜色还原的以前的颜色
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=currentcolor,this.style.fontWeight='';");
                if (e.Row.FindControl("lnkBtn") != null)
                {
                    LinkButton lnkBtn = (LinkButton)e.Row.FindControl("lnkBtn");
                    lnkBtn.Attributes.Add("onclick", "javascript:shield();");
                }
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
            if (e.CommandName == "Page") return;
            DataTable dtCollocate = (DataTable)ViewState["dtCollocate"];
            DataView dv = dtCollocate.DefaultView;
            dv.RowFilter = string.Format("match = '{0}'", e.CommandArgument);
            ViewState["KeyWords"] = e.CommandArgument.ToString();
            DataTable dtFilter = dv.ToTable();
            dtFilter.TableName = "Table-CollFilter";
            ViewState["dtCollFilter"] = dtFilter;

            divCollView.Visible = true;
            spCoLLCount.InnerHtml = string.Format("Total number of matches with <strong>\"{0}\"</strong> is <strong>{1}</strong>", e.CommandArgument, dtFilter.Rows.Count);

            if (dtFilter.Rows.Count > 100)
            {
                spCollTips.InnerHtml = string.Format("Click on the <strong>\"Title\"</strong> in each row of the list to view the corpus context;The <strong>\"View All\"</strong> Button is disabled because Total number of matches with <strong>\"{0}\"</strong> is larger than 100.", e.CommandArgument);
                btnViewColl.Enabled = false;
            }
            else
            {
                spCollTips.InnerHtml = "Click on the <strong>\"Title\"</strong> in each row of the list to view the corpus context";
                btnViewColl.Enabled = true;
            }
            divCollComputed.Visible = false;
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
                if (e.Row.FindControl("lnkBtn") != null)
                {
                    LinkButton lnkBtn = (LinkButton)e.Row.FindControl("lnkBtn");
                    lnkBtn.Attributes.Add("onclick", "javascript:shield();");
                }
            }
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                if (ViewState["HideCol"] != null)
                {
                    e.Row.Cells[int.Parse(ViewState["HideCol"].ToString())].Visible = false; //隐藏ID列,可以取得该隐藏列的信息
                }
            }
        }

        private void BtnReColl_Click(object sender, EventArgs e)
        {
            divCollocateQuery.Visible = true;

            divCollocateResult.Visible = false;
        }

        private void BtnCloseColl_Click(object sender, EventArgs e)
        {
            divCollocateQuery.Visible = false;
            divCollocateResult.Visible = true;
            divCollComputed.Visible = true;
            divCollView.Visible = false;
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
                    const string ignore = "[\r\n\t\"]";//需要替换的符号
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
            txtcontent.Value = "";//清空正文文本
            for (int i = 0; i < rbVBS.Items.Count; i++)//清除词表选择
            {
                if (rbVBS.Items[i].Selected)
                {
                    rbVBS.Items[i].Selected = false;
                }
            }
            txt_Title.Value = "";//清空正文标题
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
            DataTable dtCorpusforWordList = FSCDLL.DAL.Corpus.GetCorpusByFilterString(rowFilter).Tables[0];
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
        /// 从Lemma结果界面返回，不清空原有处理内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBackLemma_Click(object sender, EventArgs e)
        {
            if (hdfvwIndex.Value != null)
            {
                mvNeulc.ActiveViewIndex = int.Parse(hdfvwIndex.Value);
            }
            else
            {
                mvNeulc.ActiveViewIndex = 0;
            }
        }


        /// <summary>
        /// Lemma操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSubmitForLemma_Click(object sender, EventArgs e)
        {
            string txtStr = "";//正文文本
            //检验文档正文是否输入完成
            if (string.IsNullOrEmpty(txtcontent.Value)) //处理的文本还未输入
            {
                lbErr.Text = "你还未输入或导入需要处理的文本,请确认后再试！";
                txtcontent.Focus();
            }
            else
            {
                txtStr = txtcontent.Value;
                string txtfrom = hdftxtFrom.Value;
                if (txtfrom == "0")//用户输入的文本处理
                {
                    #region  保存要处理的文本：仅有处理非语料库内的文本时才做保存采集操作
                    DataTable dtCorpus = FSCDLL.DAL.Corpus.GetCorpus().Tables[0];
                    //DataView dv = dtCorpus.DefaultView;
                    //string sqlFilter= string.Format(" OriginalText = '{0}'", txtStr);
                    //DataTable dtSelect =FSCDLL.DAL.Corpus.GetCorpusByFilterString(sqlFilter).Tables[0];
                    //if (dtSelect.Rows.Count <= 0)//本次处理的文本没有保存过
                    //{
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
                        titleStr = string.Format("{0}-{1}", txt_Title.Value.Trim(), authorName);//标题
                    }
                    DataRow drCorpus = dtCorpus.Rows.Add();
                    drCorpus["Title"] = titleStr;
                    drCorpus["OriginalText"] = txtStr;
                    drCorpus["Created"] = dtNow;
                    drCorpus["Author"] = userId;
                    string splitStr = "";
                    if (CorpusName == "NEULC")
                    {
                        splitStr = ";";
                    }
                    drCorpus["Source"] = string.Format("{0}-{1}", CorpusName, authorName);//用户录入的材料，标记来源为语料库名称＋authorName
                    drCorpus["TopicID"] = splitStr + ddlTopics.SelectedValue + splitStr;
                    drCorpus["Flag"] = 3;
                    FSCDLL.DAL.Corpus.InsertCorpus(null, drCorpus);
                    dtCorpus.AcceptChanges();
                    //}
                    #endregion  保存要处理的文本：仅有处理非语料库内的文本时才做保存采集操作
                    ViewState["KeyWords"] = "";
                    ShowContext(null, txtStr, "CECR", "3");//缺省第一次采用CECR词汇表计算
                }
            }

        }
        #endregion

        #region WordList方法

        /// <summary>
        /// 返回WordList处理前界面
        /// </summary>
        /// <param name="isClear">是否清空</param>
        private void BacktoWordList(int isClear)
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
            divContextHighLight.InnerHtml = "等待处理结果中...";
            dlChart.InnerHtml = "等待处理结果中...";
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

        /// <summary>
        /// 获取语料或用户文本的基本信息并执行级别标记输出
        /// </summary>
        /// <param name="drSource">包含正文的数据行</param>
        /// <param name="strContext">要处理的文本</param>
        /// <param name="libName">词库名称</param>
        /// <param name="lastViewIndex">处理前的视图索引号</param>
        private void ShowContext(DataRow drSource, string strContext, string libName, string lastViewIndex)
        {
            dlChart.InnerHtml = "";
            divContextHighLight.InnerHtml = "";
            divContextInfo.InnerHtml = "";
            #region 0 先过滤掉网址等干扰文本
            string txtStr = strContext;
            if (drSource != null)
            {
                txtStr = SystemDataExtension.GetString(drSource, "OriginalText");
            }
            const string regEx = @"((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";
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
                lbErr.Text = "语料中不包含英文单词，无法处理！";
                return;
            }
            #endregion 1 过滤文本判断是否包含有英文单词
            else
            {
                lbErr.Text = "";
                #region 2 参照词库选择
                int maxIndex = 4;
                string libFile = string.Format("{0}words/{1}.txt", dbPath, libName);//包含原型与变型以及对应等级的词汇表
                #endregion 2 参照词库选择

                #region 3 单词还原
                Dictionary<int, object> allwordsList = WordBLL.SearchWordsWithTxt(txtlist, libFile, 0);//对词汇列表进行比对还原和级别确认，输出三个数据集：1、文本词汇对应级别，2、超纲词汇对应词频，3、处理过的单词原型对应级别
                #endregion 3 单词还原

                #region 4 WordList和结果输出
                if (allwordsList.Count > 0)
                {
                    hdfvwIndex.Value = lastViewIndex;
                    mvNeulc.ActiveViewIndex = 6;
                    List<List<string>> showWordsList = (List<List<string>>)allwordsList[0];//文本处理后包含的级别及每个级别词频的列表集合
                    DataTable dtWordsAnalysisTable = OutputResult.InitWordsAnalysisTable(showWordsList, maxIndex, symbolFile);

                    StringBuilder sb = new StringBuilder();
                    for (int k = 0; k < dtWordsAnalysisTable.Rows.Count; k++)
                    {
                        DataRow drWordsAnalysis = dtWordsAnalysisTable.Rows[k];
                        sb.Append(GetLegend(drWordsAnalysis[0].ToString(), int.Parse(drWordsAnalysis[1].ToString()), txtlist.Count));
                    }

                    dlChart.InnerHtml = sb.ToString();
                    string kWords = ViewState["KeyWords"].ToString();
                    divContextHighLight.InnerHtml = GetHighlightContext(showWordsList, kWords).ToString();
                    ViewState["LemmaContext"] = txtStr;
                    divContextInfo.InnerHtml = GetContextInfo(drSource, txtStr, kWords, libName).ToString();
                }
                else
                {
                    lbErr.Text = "语料中不包含英文单词，无法处理！";
                    return;
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




        #endregion

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
                string keyWord1 = txtfreqField1.Value.Trim();
                string keyWord2 = txtfreqField2.Value.Trim();
                if (keyWord1 == keyWord2)
                {
                    lbErr.Text = "两个相同的关键词不可比较！";
                    return;
                }
                string cTitle = "";
                string filterStr = "";
                if (rblforCompare.SelectedValue == "All" || ViewState["filterExp"] == null)
                {
                    filterStr = string.Format("Source = '{0}'", CorpusName);
                }
                else
                {
                    filterStr = ViewState["filterExp"].ToString();
                }

                DataTable dtComputed = InitComparedTable();

                cTitle += string.Format("\"{0}\"", keyWord1);
                string filterStr1 = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%' And ({1})", keyWord1, filterStr);
                DataTable dt1 = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr1).Tables[0];
                DataTable dtC1 = Common.GetWordsFromCorpus(dt1, keyWord1, 0, 0);
                GetComparedDate(ref dtComputed, dtC1, keyWord1);


                cTitle += string.Format(",\"{0}\"", keyWord2);
                string filterStr2 = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%' And {1}", keyWord2, filterStr);
                DataTable dt2 = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr2).Tables[0];
                DataTable dtC2 = Common.GetWordsFromCorpus(dt2, keyWord2, 0, 0);
                GetComparedDate(ref dtComputed, dtC2, keyWord2);

                if (!string.IsNullOrEmpty(txtfreqField3.Value.Trim()))
                {
                    string keyWord3 = txtfreqField3.Value.Trim();
                    if (keyWord3 == keyWord1 || keyWord3 == keyWord2)
                    {
                        lbErr.Text = "第三个关键词与前两个关键词某一个相同，不参与比较!";
                    }
                    else
                    {
                        cTitle += string.Format(",\"{0}\"", keyWord3);
                        string filterStr3 = string.Format("' '+OriginalText+' ' like '%[^a-zA-Z]{0}[^a-zA-Z]%' And {1}", keyWord3, filterStr);
                        DataTable dt3 = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterStr3).Tables[0];
                        DataTable dtC3 = Common.GetWordsFromCorpus(dt3, keyWord3, 0, 0);
                        GetComparedDate(ref dtComputed, dtC3, keyWord3);
                    }
                }
                divQueryforCompare.Visible = false;
                divforCompareResult.Visible = true;
                //DataTable dt = Common.TranspositionDT(dtComputed);
                BindComparedData(dtComputed, ctForCompare, cTitle + "词频与分布语篇数");
            }

        }


        private void BtnBackToCompare_Click(object sender, EventArgs e)
        {
            ctForCompare.Series.Clear();
            divQueryforCompare.Visible = true;
            divforCompareResult.Visible = false;
        }




        #endregion Compare事件
        #region Compare方法

        /// <summary>
        /// 根据用户选择的关键字，计算每个关键词出现的次数和出现该关键词的语料数
        /// </summary>
        /// <param name="dtComputed"></param>
        /// <param name="dtSource"></param>
        /// <param name="wd"></param>
        private void GetComparedDate(ref DataTable dtComputed, DataTable dtSource, string wd)
        {
            dtComputed.Columns.Add(wd);
            dtComputed.Rows[0][wd] = dtSource.Rows.Count;

            DataView dv = dtSource.DefaultView;
            DataTable dtDistinct = dv.ToTable(true, "CorpusID");

            dtComputed.Rows[1][wd] = dtDistinct.Rows.Count;
            dtComputed.AcceptChanges();
        }

        /// <summary>
        /// 比较柱状图Chart绑定数据
        /// </summary>
        /// <param name="dt">绑定的数据源</param>
        /// <param name="ct1">chart图像ID</param>
        /// <param name="ctTitle">chart图像标题</param>
        private void BindComparedData(DataTable dt, Chart ct1, string ctTitle)
        {
            ct1.Titles.Clear();
            ct1.Titles.Add(ctTitle);
            ct1.Series.Clear();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                Series cs = new Series() { ChartType = SeriesChartType.Column, Name = dt.Columns[i].ColumnName, Label = " #VAL ", IsValueShownAsLabel = true, IsXValueIndexed = false, ShadowOffset = 1, CustomProperties = "DrawingStyle=Cylinder, MinPixelPointWidth=20, MaxPixelPointWidth=35, PointWidth=0.3" };

                cs.Points.DataBind(dt.DefaultView, dt.Columns[0].ColumnName, dt.Columns[i].ColumnName, string.Format("LegendText={0},YValues={1},ToolTip={1}", dt.Columns[0].ColumnName, dt.Columns[i].ColumnName));

                Legend lg = new Legend() { Name = dt.Columns[i].ColumnName, BackColor = Color.Transparent, Docking = Docking.Bottom };
                ct1.Legends.Add(lg);
                ct1.Series.Add(cs);
            }

            ct1.DataBind();
        }

        /// <summary>
        /// 创建比较计算结果表
        /// </summary>
        /// <returns></returns>
        private DataTable InitComparedTable()
        {
            DataTable dt = new DataTable();
            //添加列
            dt.Columns.Add("Items");//比较项目
            DataRow dr = dt.NewRow();
            dr[0] = "Number of times the keyword appears";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = "Number of corpus that contains the word";
            dt.Rows.Add(dr);
            return dt;
        }

        #endregion Compare方法
        #endregion
    }
}
