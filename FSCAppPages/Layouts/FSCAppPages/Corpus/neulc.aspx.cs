using System;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data;
using System.Web.UI;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using System.Text;
using lemmatizerDLL;
using Microsoft.SharePoint;
using System.Collections;
using System.Collections.Generic;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{
    public partial class neulc : LayoutsPageBase
    {
        public ArrayList CiLib;
        #region 页面事件
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            menu01.MenuItemClick += Menu01_MenuItemClick;
            btnChange.Click += BtnChange_Click;
            //btnUploadFile.ServerClick += new EventHandler(btnUploadFile_ServerClick);
            if (!IsPostBack)
            {
                inputDiv.Visible = true;
                outputDiv.Visible = false;
                mv1.ActiveViewIndex = 0;
            }
            string WordsFile = GetDbPath() + "words/AllWords.txt";
            CiLib = WordBLL.cibiaoku(WordsFile);
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
        /// <summary>
        /// 单词统计
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
        /// 筛选词汇等级选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cblist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string clickedItem = Request.Form["__EVENTTARGET"];//得到用户点击的是哪个
            if (clickedItem.Length > clickedItem.LastIndexOf("$", StringComparison.Ordinal) + 1)
            {
                clickedItem = clickedItem.Substring(clickedItem.LastIndexOf("$") + 1);//进行拆分处理
                int thisIndex = int.Parse(clickedItem);
                if (this.cblist.Items[thisIndex].Selected)
                {
                    for (int i = 0; i <= thisIndex; i++)
                    {
                        this.cblist.Items[i].Selected = true;
                    }
                    for (int i = thisIndex + 1; i < cblist.Items.Count; i++)
                    {
                        this.cblist.Items[i].Selected = false;
                    }
                }
                else
                {
                    for (int i = 0; i < thisIndex; i++)
                    {
                        this.cblist.Items[i].Selected = true;
                    }
                    for (int i = thisIndex; i < cblist.Items.Count; i++)
                    {
                        this.cblist.Items[i].Selected = false;
                    }
                }
            }

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
            DateTime t1;//时间1
            DateTime t2;//时间2


            string titleStr;//标题
            string nameStr;//用户名
            string txtStr = this.txtcontent.Value.Trim();//正文文本

            //检验文档标题、用户名、正文是否输入完成
            if (string.IsNullOrEmpty(homecity_name.Value) || homecity_name.Value == "Type the title or click to choose it")//标题为空或者为文本框提示值,即未输入标题
            {
                PageAlert("你还未选择或输入文档标题!", this);
                homecity_name.Focus();
                lemmanew.Enabled = true;
                return;
            }
            else
            {
                titleStr = homecity_name.Value;//标题
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
            t1 = DateTime.Now;
            string ignoreWordsFile = GetDbPath() + "words/ignoreWords.txt";
            string ordinalWordsFile = GetDbPath() + "words/OrdinalWords.txt";
            string symbolFile = GetDbPath() + "words/symbol.txt";

            var txtlist = TextInput.ArticleToList(txtStr, ignoreWordsFile, ordinalWordsFile, symbolFile);//文本转化为字符串数组,将需要处理的单词存到数组中

            t2 = DateTime.Now;
            string timeRecord = "文本过滤耗时:" + TimeSpend(t1, t2);//时间记录

            //PageAlert(timeRecord, this);
            if (txtlist.Count == 0)//文本中不包含有英文单词
            {
                PageAlert("文本中不包含需要处理的英文单词！", this);
                txtcontent.Focus();
                lemmanew.Enabled = true;
                return;
            }
            #endregion

            #region 2 参照词库选择
            int itemsCount = this.cblist.Items.Count;
            int maxIndex = 0;
            for (int i = 0; i < itemsCount; i++)
            {
                if (cblist.Items[i].Selected)
                {
                    maxIndex = i + 1;//2016-10-17 13:16:43 增补高中词汇，并将级别序号变更为5、6、7、8
                }
            }
            if (maxIndex == 0)
            {
                PageAlert("你还未选定要参照的词汇表,请选择后继续!", this);
                lemmanew.Enabled = true;
                return;
            }
            #endregion

            #region 3 保存要处理的文本
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            //string loginName = currentUser.LoginName;
            //loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            //loginName = loginName.Replace(@"i:0#.w|", "");
            string spName = currentUser.Name;
            if (nameStr != spName)
            {
                nameStr = string.Format("{0}_{1}", nameStr, spName);
            }
            titleStr = TextInput.FilterSpecial(titleStr, "");
            string filePath = GetDbPath() + @"export/";//txt文件保存的路径
            string nowStr = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now);//时间格式字符串：年月日时分秒4位毫秒
            string fileTitle = string.Format("{0}({1}){2}.txt", titleStr, nameStr, nowStr); ;//文章标题+ _ + 处理人姓名 + 处理人所属院校（登录名）+ 当前时间
            t1 = DateTime.Now;
            TextInput.FileWrite(fileTitle, txtStr, filePath);//将即将处理的文本保存到服务器上的指定目录中;
            t2 = DateTime.Now;
            timeRecord = string.Format("{0}文件保存时间耗时:{1}", timeRecord, TimeSpend(t1, t2));

            //PageAlert(timeRecord, this);
            inputDiv.Visible = false;
            #endregion

            #region 4 单词还原
            t1 = DateTime.Now;
            string fileName = GetDbPath() + "words/AllWords.txt";//包含原型与变型以及对应等级的词汇表
            int isEurope = 0;
            //if (ckEurope.Checked)
            //{
            //    isEurope = 1;
            //}
            Dictionary<int, object> allwordsList = WordBLL.SearchWordsWithTxt(txtlist, fileName, isEurope);//对词汇列表进行比对还原和级别确认，输出三个数据集：1、文本词汇对应级别，2、超纲词汇对应词频，3、处理过的单词原型对应级别
            t2 = DateTime.Now;
            timeRecord = string.Format("{0}Lemma耗时:{1}", timeRecord, TimeSpend(t1, t2));
            #endregion

            #region 5 Lemma和结果输出
            if (allwordsList.Count > 0)
            {
                #region 5.1 输出词汇表
                t1 = DateTime.Now;
                DataTable showWordsdt = (DataTable)allwordsList[1];
                //DataTable  = OutputResult.newDataTable(wordsdt, maxIndex);
                //gridview数据绑定
                #region 5.1.1 无超纲或不可处理的词汇可输出
                if (showWordsdt.Rows.Count == 0)
                {
                    showWordsdt.Rows.Add(showWordsdt.NewRow());
                    wordgv.DataSource = showWordsdt;
                    wordgv.DataBind();
                    int nColumnCount = wordgv.Rows[0].Cells.Count;
                    wordgv.Rows[0].Cells.Clear();
                    using (var tableCell = new TableCell())
                    {
                        wordgv.Rows[0].Cells.Add(tableCell);
                    }
                    wordgv.Rows[0].Cells[0].ColumnSpan = nColumnCount;
                    wordgv.Rows[0].Cells[0].Text = "本次处理的文档不包含超纲或无法处理的词汇！";
                    wordgv.RowStyle.Height = 30;
                    wordgv.RowStyle.HorizontalAlign = HorizontalAlign.Center;
                }
                #endregion
                #region 5.1.2 有超纲或不可处理的词汇输出词表
                else
                {
                    DataView dv = showWordsdt.Copy().DefaultView;
                    dv.RowFilter = "[level]<=0 or [level]>" + (maxIndex + 4); //筛选超纲词汇和未处理词汇
                                                                              //先给它一个默认状态，因为下面的bin()方法，需要用到状态值。
                    ViewState["SortOrder"] = "Frequency";
                    ViewState["OrderDire"] = "Desc";
                    string sortStr = string.Format("{0} {1}", (string)ViewState["SortOrder"], (string)ViewState["OrderDire"]);
                    dv.Sort = sortStr;
                    wordgv.DataSource = dv;
                    totalW.Text = string.Format("Total:{0}Words", dv.Table.Rows.Count);
                    wordgv.DataBind();
                    t2 = DateTime.Now;
                    timeRecord = string.Format("{0}词表输出：{1}", timeRecord, TimeSpend(t1, t2));
                    /***************5.3.1 输出词汇级别分布饼图*****************/
                    //int totalwords = showWordsdt.Rows.Count;
                    //int[] wordProfiles =OutputResult.WordProfiles(showWordsdt, maxIndex);
                    //Drawpie(wordProfiles,maxIndex);
                    //t1 = DateTime.Now;
                    //timeRecord = timeRecord + "级别分布图输出：" + TimeSpend(t2, t1);
                    /***************5.3.2 输出前十个最高频词频分布饼图*********************/
                    //DrawSequencePie(showWordsdt);
                    //t2 = DateTime.Now;
                    //timeRecord = timeRecord + "高频词频分布图输出：" + TimeSpend(t1, t2);

                }
                #endregion
                #endregion
                /*****************5.2 输出彩色标记文本*************************/
                var showWordsList = (List<List<string>>)allwordsList[0];//文本处理后包含的级别及每个级别词频的列表集合
                DataTable dt = OutputResult.InitWordsAnalysisTable(showWordsList, maxIndex, symbolFile);
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    sb.Append(OutputResult.Colored(string.Format("{0}({1}) ", dr[0], dr[1]), int.Parse(dr[2].ToString())));
                }

                tuliDiv.InnerHtml = sb.ToString();//OutputResult.Tuli(showWordsList, maxIndex, symbolFile); //输出图例颜色表
                BindChart(dt, SeriesChartType.Pie, Chart1);
                //outDiv.InnerHtml = OutputResult.ResultDiv(showWordsList, maxIndex); //输出彩色文本
                outDiv.InnerHtml = OutputResult.ResultDiv(showWordsList, maxIndex).ToString();
                outDiv.Visible = true;
                outlb.Text = titleStr;
                t1 = DateTime.Now;
                timeRecord = string.Format("{0}彩色标记文本输出：{1}", timeRecord, TimeSpend(t2, t1));


                //PageAlert(timeRecord,this);
                outputDiv.Visible = true;

                //OutputResult.VisibleOrNot(outputDiv,inputDiv);
                //ClientScript.RegisterStartupScript(ClientScript.GetType(), "", "<script>document.getElementById('inputDiv').style.display = 'none';document.getElementById('outputDiv').style.display = '';</script>", true);//隐藏输入界面，显示输出结果
                Titlelb.Text = "Output";

                //Dictionary<string, int> wordsTimes =(Dictionary<string, int>) ret[2];
                //WordBLL.WriteIntoDB(wordsTimes);
            }
            lemmanew.Enabled = true;
            #endregion
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void closeBtn_OnClick(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", "<script>document.getElementById('outputDiv').style.display = '';document.getElementById('inputDiv').style.display = 'none';</script>", true);
            this.txtcontent.Value = "";//清空正文文本
            for (int i = 0; i < this.cblist.Items.Count; i++)//清除词表选择
            {
                if (this.cblist.Items[i].Selected)
                {
                    this.cblist.Items[i].Selected = false;
                }
            }
            Titlelb.Text = "Input";
            homecity_name.Value = "";//清空正文标题
                                     //this.usertb.Text = "";//清空操作者用户名,默认不清空
            lemmanew.Enabled = true;
            outputDiv.Visible = false;
            inputDiv.Visible = true;
            //OutputResult.VisibleOrNot(inputDiv,outputDiv);
        }

        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void backBtn_OnClickBtn_OnClick(object sender, EventArgs e)
        {
            outputDiv.Visible = false;
            inputDiv.Visible = true;
            Titlelb.Text = "Input";
        }

        /// <summary>
        /// 词汇表数据行创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void wordgv_OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow ||
                e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[2].Visible = false; //如果想使第1列不可见,则将它的可见性设为false
            }
        }

        /// <summary>
        /// 词汇表数据行绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void wordgv_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex == -1)
            {
                e.Row.Cells[0].Text = "NO.";
                e.Row.Cells[1].Text = "Word";
                e.Row.Cells[2].Text = "Level";
                e.Row.Cells[3].Text = "Frequency";

            }
            else
            {
                int indexId = e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = indexId.ToString();
            }

        }

        /// <summary>
        /// 清空按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearBtn_OnClick(object sender, EventArgs e)
        {
            this.txtcontent.Value = "";
        }

        /// <summary>
        /// 词汇表排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void wordgv_OnSorting(object sender, GridViewSortEventArgs e)
        {
            string sPage = e.SortExpression;
            if (ViewState["SortOrder"].ToString() == sPage)
            {
                if (ViewState["OrderDire"].ToString() == "Desc")
                {
                    ViewState["OrderDire"] = "ASC";
                }
                else
                {
                    ViewState["OrderDire"] = "Desc";
                }
            }
            else
            {
                ViewState["SortOrder"] = e.SortExpression;
            }
            wordgv.DataBind();
        }

        /// <summary>
        /// 文本导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtImport_Click(object sender, EventArgs e)
        {
            string fileName = ImportTxt();
            if (fileName.Length > 0)
            {
                string txt = TextInput.ReadTxt(fileName);

                this.txtcontent.Value = txt;
            }
            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", "<script>wordStatic(txtcontent);</script>", true);
        }
        
        #endregion
        private void BtnChange_Click(object sender, EventArgs e)
        {
            if (lbAA.Text == "现在是开灯状态")
            {
                btnChange.BackColor = Color.Black;
                btnChange.ForeColor = Color.White;
                btnChange.Text = "开灯";
                lbAA.Text = "现在是关灯状态";
            }
            else
            {
                btnChange.BackColor = Color.White;
                btnChange.ForeColor = Color.Black;
                btnChange.Text = "关灯";
                lbAA.Text = "现在是开灯状态";
            }
        }

        private DataTable GetCorpus()
        {
            DataTable dtCorpus = new DataTable();
            return dtCorpus;
        }

        private void Menu01_MenuItemClick(object sender, MenuEventArgs e)
        {
            switch (menu01.SelectedValue)
            {
                case "1"://这个值是在Menu中加入Item时设定的
                    {
                        mv1.ActiveViewIndex = 1;
                        break;
                    }
                case "2":
                    {
                        mv1.ActiveViewIndex = 2;
                        break;
                    }
                case "3":
                    {
                        mv1.ActiveViewIndex = 3;
                        break;
                    }
                case "4":
                    {
                        mv1.ActiveViewIndex = 4;
                        break;
                    }
                default:
                    mv1.ActiveViewIndex = 0;
                    break;
            }
        }

        #region WordList
        #region 方法

        private static void BindChart(DataTable dt, SeriesChartType sctype, Chart chart)
        {
            string colX = dt.Columns[0].ColumnName;
            string colY = dt.Columns[1].ColumnName;
            chart.Series[0].Points.DataBind(dt.DefaultView, colX, colY, string.Format("LegendText={0},YValues={1},ToolTip={0}", colX, colY));
            chart.Series[0].ChartType = sctype;
            chart.Series[0].IsValueShownAsLabel = true;
            chart.Series[0].Color = Color.FromArgb(60, 255, 0, 0);
            chart.Series[0].BorderColor = Color.Transparent;
            chart.Series[0].BorderWidth = 1;
            chart.Series[0].LabelForeColor = Color.White;

            chart.Titles.Clear();
            chart.Titles.Add("词级-词汇量分布图");


            //背景色设置
            chart.ChartAreas[0].ShadowColor = Color.Transparent;
            chart.ChartAreas[0].BackColor = Color.FromArgb(209, 237, 254);         //该处设置为了由天蓝到白色的逐渐变化
            chart.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
            chart.ChartAreas[0].BackSecondaryColor = Color.White;
            //中间X,Y线条的颜色设置
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);

            //图例
            Legend lgnd = new Legend
            {
                Enabled = true,
                BackColor = Color.Transparent,
                Docking = Docking.Top,
                Alignment = StringAlignment.Center
            };
            chart.Legends.Clear();
            chart.Legends.Add(lgnd);

            // Show as 3D
            chart.ChartAreas[0].Area3DStyle.Enable3D = false;
            //binding data
            chart.DataBind();
        }


        private string GetDbPath()
        {
            string txtPath = Server.MapPath("");
            txtPath = txtPath.Substring(0, txtPath.IndexOf("\\layouts", StringComparison.Ordinal)) + @"\layouts\db\";
            return txtPath;
        }
        private static string TimeSpend(DateTime startTime, DateTime endTime)
        {
            TimeSpan ts = endTime - startTime;
            string rtime = string.Format("{0}:{1}:{2}:{3} : {4}:", ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            return rtime;
        }


        /// <summary>
        /// 去掉文件名中的特殊符号
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetFileName(string fileName)
        {
            string retDate = Regex.Replace(fileName, @"[.#：]", "").TrimEnd('-');
            return retDate;

        }

        /// <summary>
        /// 导入txt文件
        /// </summary>
        private string ImportTxt()
        {
            string txtPath = GetDbPath() + "import/";
            string retTxt = "";
            if (txtUpload.HasFile == false)
            {
                PageAlert("选择导入的文件", this);
                retTxt = "";
            }
            else
            {
                var extension = Path.GetExtension(txtUpload.FileName);
                if (extension != null)
                {
                    string fileExtension = extension.ToLower();
                    if (fileExtension != ".txt")
                        PageAlert("只能导入文本(.txt)文件", this);
                    else
                    {
                        //lblMsg.Text = "导入的文件：" + fileUpload.PostedFile.FileName;
                        string fileName = txtPath + GetFileName(txtUpload.FileName.Replace(fileExtension, "")) + fileExtension;
                        try
                        {
                            txtUpload.PostedFile.SaveAs(fileName);
                            retTxt = fileName;
                        }
                        catch (Exception ex)
                        {
                            PageAlert("文件导入失败" + ex.ToString(), this);
                            retTxt = "";
                        }

                    }
                }
            }
            return retTxt;

        }

        protected void Drawpie(int[] dataInts, int maxIndex)
        {
            string[] textStrings = new string[maxIndex + 2];
            string[] colorStrings = new string[maxIndex + 2];
            if (maxIndex == 1)
            {
                textStrings = new string[3] { "超纲词汇", "基本要求", "未处理词汇" };
                colorStrings = new string[3] { "#FF0000", "#7FFF00", "#1C1C1C" };
            }
            else if (maxIndex == 2)
            {
                textStrings = new string[4] { "超纲词汇", "基本要求", "较高要求", "未处理词汇" };
                colorStrings = new string[4] { "#FF0000", "#7FFF00", "#FFFF00", "#1C1C1C" };
            }
            else if (maxIndex == 3)
            {
                textStrings = new string[5] { "超纲词汇", "基本要求", "较高要求", "更高要求", "未处理词汇" };
                colorStrings = new string[5] { "#FF0000", "#7FFF00", "#FFFF00", "#FF7F50", "#1C1C1C" };
            }
            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", string.Format("<script>drawCircle('canvas_lv', {0},{1}, {2});</script>", dataInts, colorStrings, textStrings), true);
        }

        protected void DrawSequencePie(DataTable dtTable)
        {
            DataView dv = dtTable.DefaultView;
            dv.Sort = "Frequency DeSC";
            DataTable dtTableCopy = dv.ToTable();
            int rows = dtTableCopy.Rows.Count;
            if (rows > 0 && rows <= 10) //
            {
                int[] dataInts = new int[rows];
                string[] textStrings = new string[rows];
                string[] colorStrings = new string[rows];
                for (int i = 0; i < rows; i++)
                {
                    dataInts[i] = int.Parse(dtTableCopy.Rows[i][2].ToString());
                    textStrings[i] = dtTableCopy.Rows[i][0].ToString();
                    colorStrings[i] = OutputResult.GetRandomColor();
                }
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "", string.Format("<script>drawCircle('canvas_seq', {0},{1}, {2});</script>", dataInts, colorStrings, textStrings), true);
            }
            else
            {
                int[] dataInts = new int[11];
                string[] textStrings = new string[11];
                string[] colorStrings = new string[11];
                int k = 0;
                for (int i = 0; i < 10; i++)
                {
                    dataInts[i] = int.Parse(dtTableCopy.Rows[i][2].ToString());
                    textStrings[i] = dtTableCopy.Rows[i][0].ToString();
                    colorStrings[i] = OutputResult.GetRandomColor();
                }
                for (int i = 10; i < rows; i++)
                {
                    k = k + int.Parse(dtTableCopy.Rows[i][2].ToString());
                }
                colorStrings[10] = OutputResult.GetRandomColor();
                dataInts[10] = k;
                textStrings[10] = "Other Words";
                ClientScript.RegisterStartupScript(ClientScript.GetType(), "", string.Format("<script>drawCircle('canvas_seq', {0},{1}, {2});</script>", dataInts, colorStrings, textStrings), true);
            }
        }

        protected void exExcel_OnClick(object sender, EventArgs e)
        {

            //ExportToExcel.ExportExcel(dt)

            string title = string.Format("{0}({1})_", this.homecity_name.Value, username.Value);
            DateTime dt = DateTime.Now;
            string excelTitle = title + dt.ToString("yyyyMMddhhmmss");
            excelTitle = excelTitle + ".xls";
            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;//注意编码
            HttpContext.Current.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(excelTitle, Encoding.UTF8).ToString());
            HttpContext.Current.Response.ContentType = excelTitle;//image/JPEG;text/HTML;image/GIF;vnd.ms-excel/msword
            wordgv.Page.EnableViewState = false;
            StringWriter tw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            wordgv.RenderControl(hw);
            HttpContext.Current.Response.Write(tw.ToString());
            HttpContext.Current.Response.End();
            //GridViewToExcel(wordgv, "application/ms-excel", excelTitle);
            // Export(gvRecord, "application/ms-excel", str);
        }

        /// <summary>
        /// 将网格数据导出到Excel
        /// </summary>
        /// <param name="ctrl">网格名称(如GridView1)</param>
        /// <param name="fileType">要导出的文件类型(Excel:application/ms-excel)</param>
        /// <param name="fileName">要保存的文件名</param>
        public static void GridViewToExcel(Control ctrl, string fileType, string fileName)
        {
            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;//注意编码
            HttpContext.Current.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8).ToString());
            HttpContext.Current.Response.ContentType = fileType;//image/JPEG;text/HTML;image/GIF;vnd.ms-excel/msword
            ctrl.Page.EnableViewState = false;
            StringWriter tw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            ctrl.RenderControl(hw);
            HttpContext.Current.Response.Write(tw.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// ReLoad this VerifyRenderingInServerForm is neccessary,虽然没有执行代码，但是必须的
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {
        }
        #endregion
        #endregion
    }
}
