using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;

namespace FSCWebParts.Corpus
{
    public partial class CorpusUserControl : UserControl
    {
        #region Members

        public Corpus webObj;
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            cblQueryTopics.SelectedIndexChanged += CblQueryTopics_SelectedIndexChanged;
            cblQueryStyle.SelectedIndexChanged += CblQueryStyle_SelectedIndexChanged;
            gvCorpus.RowDataBound += GvCorpus_RowDataBound;
            tbKeyWord.TextChanged += TbKeyWord_TextChanged;
            btnQuery.Click += BtnQuery_Click;
            btnQuery.Enabled = true;
            if (string.IsNullOrEmpty(tbKeyWord.Text.Trim()))
                btnQuery.Enabled = false;
            if (!IsPostBack)
            {
                DataTable dtCorpus = GetCorpus();
                dtCorpus.TableName = "Corpus";
                ViewState["dtCorpus"] = dtCorpus;
                //SetPage();
                //GridViewDataBind("");//调用数据绑定函数DataBind()进行数据绑定运算
                InitGridView(dtCorpus);

                DataTable dtGrade = GetDT("Grade", 5);
                dtGrade.TableName = "Grade";
                ViewState["Grade"] =dtGrade;
                BindDDL(ddlGrade,dtGrade);

                DataTable dtStyle = GetDT("Style", 7);
                dtStyle.TableName = "Style";
                ViewState["Style"] =dtStyle;
                BindCbl(cblQueryStyle,dtStyle);

                DataTable dtTopic = GetDT("Topic", 9);
                dtTopic.TableName = "Topic";
                ViewState["Topic"] =dtTopic;
                BindCbl(cblQueryTopics, dtTopic);

                BindCbl(cblLiterary, dtStyle);
                BindCbl(cblTopics, dtTopic);
            }
        }

        private void TbKeyWord_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbKeyWord.Text.Trim()))
            {
                Response.Write("<script>alert('尚未输入检索关键词！')</script");
                tbKeyWord.Focus();
                return;
            }
            else
            {
                string keyWords = tbKeyWord.Text.Trim();
                DataTable dtCorpus = (DataTable)ViewState["dtCorpus"];
                DataView dv = dtCorpus.DefaultView;
                dv.RowFilter = string.Format("Context Like '%{0}%' or Context Like '%{0} %' or Context Like '% {0}%' ", keyWords);
                DataTable dtFiltered = dv.ToTable();
                InitGridView(dtFiltered);
            }
        }

        private void CblQueryTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dtCorpus = (DataTable)ViewState["dtCorpus"];
                DataTable dtQueried = GetQueriedTableByCBL(cblQueryStyle, dtCorpus, "StyleID");
                DataTable dtQueried2 = GetQueriedTableByCBL(cblQueryTopics, dtQueried, "TopicID");
                InitGridView(dtQueried);
            }
            catch (Exception ex)
            {
                lbErr.Text = ex.ToString();
            }

        }

        private void CblQueryStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dtCorpus = (DataTable)ViewState["dtCorpus"];
                DataTable dtQueried = GetQueriedTableByCBL(cblQueryStyle,dtCorpus,"StyleID");
                DataTable dtQueried2 = GetQueriedTableByCBL(cblQueryTopics, dtQueried,"TopicID");
                InitGridView(dtQueried);
            }
            catch (Exception ex)
            {
                lbErr.Text = ex.ToString();
            }

        }

        private DataTable GetQueriedTableByCBL(CheckBoxList cbl,DataTable dtSource,string filterCol)
        {
            DataTable dt = new DataTable();
            string ckValue = "";
            var builder = new StringBuilder();
            builder.Append(ckValue);
            for (int i = 0; i < cbl.Items.Count; i++)
            {
                if (cbl.Items[i].Selected)
                {
                    builder.Append(cbl.Items[i].Value + ";");
                }
            }
            ckValue = builder.ToString();
            if (ckValue != "")
            {
                ckValue = ckValue.TrimEnd(';');
                ckValue = ckValue.Replace(";", string.Format(" Or {0} = ", filterCol));
                ckValue = string.Format("{0} = {1}", filterCol, ckValue);
                DataView dv = dtSource.DefaultView;
                dv.RowFilter = ckValue;
                dt = dv.ToTable();
            }
            else
            {
                dt=dtSource.Copy();
            }
            return dt;
        }

        private void InitGridView(DataTable dtCorpus)
        {
            try
            {
                gvCorpus.DataSource = dtCorpus;
                gvCorpus.DataBind();
            }
            catch (Exception ex)
            {
                lbErr.Text = ex.ToString();
            }

        }

        private void GvCorpus_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string title = "";
                    int Id = 0;
                    if (e.Row.FindControl("lbContext") != null)//高亮关键字
                    {
                        Label lbContext = (Label)e.Row.FindControl("lbContext");
                        string keyWords = tbKeyWord.Text.Trim();
                        string inputText = lbContext.Text;
                        lbContext.Text = ReplaceRed(inputText, keyWords);
                    }
                    //if (e.Row.FindControl("lbStyle") != null && e.Row.FindControl("hdfStyleId") != null)
                    //{
                    //    HiddenField hdfStyleId = (HiddenField)e.Row.FindControl("hdfStyleId");
                    //    Label lbStyle = (Label)e.Row.FindControl("lbStyle");
                    //    DataTable dtStyle = (DataTable)ViewState["Style"];
                    //    dtStyle.TableName = "Style";
                    //    if(hdfStyleId.Value!="")
                    //        Id = int.Parse(hdfStyleId.Value);
                    //    title = GetTitleByID(dtStyle, Id);
                    //    lbStyle.Text = title;
                    //}
                    //if (e.Row.FindControl("lbTopic") != null && e.Row.FindControl("hdfTopicId") != null)
                    //{
                    //    HiddenField hdfTopicId = (HiddenField)e.Row.FindControl("hdfTopicId");
                    //    Label lbTopic = (Label)e.Row.FindControl("lbTopic");
                    //    DataTable dtTopic = (DataTable)ViewState["Topic"];
                    //    dtTopic.TableName = "Topic";
                    //    if (hdfTopicId.Value != "")
                    //        Id = int.Parse(hdfTopicId.Value);
                    //    title = GetTitleByID(dtTopic, Id);
                    //    lbTopic.Text = title;
                    //}

                    //设置悬浮鼠标指针形状为"小手"
                    e.Row.Attributes["style"] = "Cursor:hand";
                }
            }
            catch (Exception ex)
            {
                lbErr.Text = ex.ToString();
            }

            //string strGvName = "gvCorpus";
            //e.Row.Attributes.Add("id", strGvName + _i.ToString());
            //e.Row.Attributes.Add("onKeyDown", "SelectRow(event,'" + strGvName + "');");
            //e.Row.Attributes.Add("onClick", "MarkRow(" + _i.ToString() + ",'" + strGvName + "');");
            //e.Row.Attributes.Add("tabindex", "0");
            //_i++;
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuery_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbKeyWord.Text.Trim()))
            {
                Response.Write("<script>alert('尚未输入检索关键词！')</script");
                tbKeyWord.Focus();
                return;
            }
            else
            {
                string keyWords = tbKeyWord.Text.Trim();
                DataTable dtCorpus = (DataTable)ViewState["dtCorpus"];
                DataView dv = dtCorpus.DefaultView;
                dv.RowFilter = string.Format("Context Like '%{0}%' or Context Like '%{0} %' or Context Like '% {0}%' ", keyWords);
                DataTable dtFiltered = dv.ToTable();
                InitGridView(dtFiltered);
            }
        }
        protected string HighlightText(string inputText, string searchWord)
        {
            System.Text.RegularExpressions.Regex expression = new System.Text.RegularExpressions.Regex(searchWord.Replace(" ", "|"), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return expression.Replace(inputText, new System.Text.RegularExpressions.MatchEvaluator(ReplaceKeywords));
        }
        public string ReplaceKeywords(System.Text.RegularExpressions.Match m)
        {
            return "<span class='highlightTxtSearch'>" + m.Value + "</span>";
        }
        public static string ReplaceRed(string inputText, string redkey)
        {
            if (redkey == "" || redkey == null)
            {
                return inputText;
            }
            else
                inputText = inputText.Replace(redkey, string.Format(" <span style='color:#ff0000'>{0}</span>", redkey));
            return inputText;
        }
        #region 临时构造数据表

        private string GetTitleByID(DataTable dt,int Id)
        {
            string title = "";
            string dtName = dt.TableName;
            string filterExp = string.Format("{0}ID = {1}", dtName, Id);
            DataRow[] drs = dt.Select(filterExp);
            if (drs.Length > 0)
                title = drs[0]["Title"].ToString();
            return title;
        }

        private void BindDDL(DropDownList DDL, DataTable dtSource)
        {
            DDL.Items.Clear();
            DDL.DataSource = dtSource;
            DDL.DataTextField = dtSource.Columns[1].ColumnName;
            DDL.DataValueField = dtSource.Columns[0].ColumnName;
            DDL.DataBind();
        }

        private void BindCbl(CheckBoxList cbl,DataTable dtSource)
        {
            cbl.Items.Clear();
            cbl.DataSource = dtSource;
            cbl.DataTextField = dtSource.Columns[1].ColumnName;
            cbl.DataValueField = dtSource.Columns[0].ColumnName;
            cbl.DataBind();
        }

        private DataTable GetCorpus()
        {
            DataTable dtCorpus = new DataTable();
            dtCorpus.Columns.Add("ID");
            dtCorpus.Columns.Add("Title");
            dtCorpus.Columns.Add("GradeID");
            dtCorpus.Columns.Add("TopicID");
            dtCorpus.Columns.Add("StyleID");
            dtCorpus.Columns.Add("Context");
            dtCorpus.Columns.Add("CodedContext");
            DataRow dr;
            for (int i = 1; i <=100; i++)
            {
                dr = dtCorpus.NewRow();
                dr["ID"] = i + 1;
                dr["Title"] = "Corpora"+i;
                dr["GradeID"] = i%5+1;
                dr["StyleID"] = i%7+1;
                dr["TopicID"] = i%9+1;
                if(i%2==0)
                {
                    dr["Context"] = "It is reported that the number of teenage smokers has been on the rise in recent years";
                }
                else
                {
                    dr["Context"] = "which has caused great concern among people in ail walks or life.";
                }
                dtCorpus.Rows.Add(dr);
            }
            return dtCorpus;

        }

        private DataTable GetDT(string tbName,int rows)
        {
            DataTable dt = new DataTable(tbName);
            dt.Columns.Add(tbName+"ID");
            dt.Columns.Add("Title");
            DataRow dr;
            for (int i = 1; i <=rows; i++)
            {
                dr = dt.NewRow();
                dr[0] = i ;
                dr[1] = tbName + i.ToString();
                dt.Rows.Add(dr);

            }
            return dt;
        }
        #endregion

    }
}
