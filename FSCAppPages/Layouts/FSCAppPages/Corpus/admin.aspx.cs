using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{/// <summary>
/// 语料的编辑、添加和删除
/// </summary>
    public partial class admin : LayoutsPageBase
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (CorpusSource == "LC")
                {
                    divQuery.Visible = true;
                    divQueryAC.Visible = false;
                    InitControls();
                }
                else
                {
                    divQuery.Visible = false;
                    divQueryAC.Visible = true;
                    InitControlsAC();
                }
                txtSource.Text = CorpusSource;
                txtSourceAc.Text = CorpusSource;
                txtSourceAc.ReadOnly = true;
                txtSource.ReadOnly = true;
                divEditCorpora.Visible = false;
                divEditCorporaAc.Visible = false;
            }
            ddlQueryMajor.SelectedIndexChanged += DdlQueryMajor_SelectedIndexChanged;
            ddlQueryMajor.AutoPostBack = true;
            ddlMajor.AutoPostBack = true;
            ddlMajor.SelectedIndexChanged += DdlQueryMajor_SelectedIndexChanged;
            btnAdd.Click += BtnAdd_Click;
            btnQuery.Click += BtnQuery_Click;
            btnSubmit.Click += BtnSubmit_Click;
            btnCancel.Click += BtnCancel_Click;
            btnQueryAc.Click += BtnQuery_Click;
            btnSubmitAc.Click += BtnSubmit_Click;
            btnCancelAc.Click += BtnCancel_Click;

            gvCorpus.RowCommand += GvCorpus_RowCommand;
            gvCorpus.RowDataBound += GvCorpus_RowDataBound;
            gvCorpus.PageIndexChanging += GvCorpus_PageIndexChanging;
        }

        /// <summary>
        /// 专业改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdlQueryMajor_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindJournal(((DropDownList)sender).SelectedValue);
        }

        private string CorpusSource
        {
            get
            {
                return Request.QueryString["Source"]==null?"LC":Request.QueryString["Source"].ToString(); 
            }
        }

        private void GvCorpus_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCorpus.PageIndex = e.NewPageIndex;
            ReQuery();
        }

        private void GvCorpus_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable dtExtend;
            if (ViewState["dtCorpusExtend"] == null)
            {
                dtExtend = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(null).Tables[0];
                ViewState["dtCorpusExtend"] = dtExtend.Copy();
            }
            else
            {
                dtExtend = (ViewState["dtCorpusExtend"] as DataTable).Copy();
            }

            foreach (GridViewRow gRow in gvCorpus.Rows)
            {
                if (gRow.RowType == DataControlRowType.DataRow)
                {
                    Label lblContext = gRow.FindControl("lbContext") as Label;
                    if (lblContext.Text.Length > 150)
                    {
                        lblContext.Text = lblContext.Text.Substring(0, 100) + "……";
                    }

                    HiddenField hId = gRow.FindControl("hdfLevelId") as HiddenField;
                    Label lblLevel;
                    if (hId.Value.Length > 0)
                    {
                        lblLevel = gRow.FindControl("lbLevel") as Label;
                        if (CorpusSource == "LC")
                            lblLevel.Text = Common.GetTitlesByIDs(dtExtend, hId.Value, "Level", split);
                        else
                            lblLevel.Text = hId.Value;
                    }
                    hId = gRow.FindControl("hdfGenreId") as HiddenField;
                    if (hId.Value.Length > 0)
                    {
                        lblLevel = gRow.FindControl("lbGenre") as Label;
                        if (CorpusSource == "LC")
                            lblLevel.Text = Common.GetTitlesByIDs(dtExtend, hId.Value, "Genre", split);
                        else
                            lblLevel.Text = hId.Value;
                    }
                    hId = gRow.FindControl("hdfTopicId") as HiddenField;
                    if (hId.Value.Length > 0)
                    {
                        lblLevel = gRow.FindControl("lbTopic") as Label;
                        if (CorpusSource == "LC")
                            lblLevel.Text = Common.GetTitlesByIDs(dtExtend, hId.Value, "Topic", split);
                        else
                            lblLevel.Text = hId.Value;
                    }
                }
            }
        }
        private void GvCorpus_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long corpusID = long.Parse(e.CommandArgument.ToString());
            //此处为查询的结果
            DataTable dt = (DataTable)ViewState["dtCorpus"];
            DataRow dr = dt.Select("CorpusID=" + corpusID)[0];
            if (e.CommandName == "EditPlan")
            {
                FillControls(dr);
                divList.Visible = false;
                divEditCorpora.Visible = true;
                ViewState["Edit"] = corpusID;
            }
            else if (e.CommandName == "DelPlan")
            {
                dr["Flag"] = 0;
                FSCDLL.DAL.Corpus.UpdateCorpus(null, dr);
                dt.Rows.Remove(dr);//从中删除
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            divList.Visible = true;
            divEditCorpora.Visible = false;

        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {

                SaveCorpus();
                ClearControls();
                if (ViewState.ToString() != "0")//如果是编辑，则返回到查询
                {
                    divEditCorpora.Visible = false;
                    divList.Visible = true;
                    ReQuery();
                }
            }
            catch (Exception ex)
            {
                lbErr.Text = ex.ToString();
            }
        }
        /// <summary>
        /// 重新查询并绑定，从编辑返回时，如果有编辑时，则重新绑定
        /// </summary>
        private void ReQuery()
        {
            DataTable dtAll;
            if (ViewState["dtCorpus"] == null)
            {
                dtAll = FSCDLL.DAL.Corpus.GetCorpus().Tables[0];
                ViewState["dtCorpus"] = dtAll;
            }
            else
            {
                dtAll = (DataTable)ViewState["dtCorpus"];
            }
            string filterExpression = GetSelectQuery();
            DataSet dsResult;
            dsResult = FSCDLL.DAL.Corpus.GetCorpusByFilterString(filterExpression);
            gvCorpus.DataSource = dsResult.Tables[0].DefaultView;
            gvCorpus.DataBind();
        }
        //查询
        private void BtnQuery_Click(object sender, EventArgs e)
        {
            ViewState["filterExp"] = null;
            ReQuery();//需要重新构造查询字符串
        }

        //添加一条新的语料
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (CorpusSource == "LC")
            {
                divEditCorpora.Visible = true;
                divEditCorporaAc.Visible = false;
            }
            else
            {
                divEditCorporaAc.Visible = true;
                divEditCorpora.Visible = false;
            }
            divList.Visible = false;
            ViewState["Edit"] = "0";
        }
        #endregion
        #region 方法
        //添加新的语料时先清空原有的控件
        private void ClearControls()
        {
            if (CorpusSource == "LC")
            {
                foreach (ListItem itm in cblLevel.Items)
                {
                    itm.Selected = false;
                }

                foreach (ListItem itm in cblGenre.Items)
                {
                    itm.Selected = false;
                }

                foreach (ListItem itm in cblTopics.Items)
                {
                    itm.Selected = false;
                }

                txtTitle.Text = "";
                txtOriginalText.Text = "";
                txtSource.Text = "";
                txtKeyWord.Text = "";
            }
            else
            {
                txtTitleAc.Text = "";
                txtOriginalTextAc.Text = "";
                txtKeyWordAc.Text = "";
                txtSourceAc.Text = "";
            }
        }
        //数据分隔符，用来分隔外键的ID值
        private const string split = ";";
        /// <summary>
        /// 编辑时填充数据
        /// </summary>
        /// <param name="drCorpus">语料数据行</param>
        private void FillControls(DataRow drCorpus)
        {
            txtTitle.Text = drCorpus["Title"].ToString();
            txtSource.Text = drCorpus["Source"].ToString();
            txtOriginalText.Text = drCorpus["OriginalText"].ToString();
            if (CorpusSource == "LC")
            {
                Common.SetCBListChecked(cblTopics, drCorpus["TopicID"].ToString(), split);
                Common.SetCBListChecked(cblLevel, drCorpus["LevelID"].ToString(), split);
                Common.SetCBListChecked(cblGenre, drCorpus["GenreID"].ToString(), split);
            }
            else//AC
            {
                ddlYear.SelectedValue = drCorpus["LevelID"].ToString();
                ddlMajor.SelectedValue = drCorpus["TopicID"].ToString();
                ddlJournal.SelectedValue = drCorpus["GenreID"].ToString();
            }
        }
        private void InitControlsAC()
        {
            DataSet ds = FSCDLL.DAL.Corpus.GetCorpusYearsOfAC();
            ddlQueryYear.DataSource = ds.Tables[0].DefaultView;
            ddlQueryYear.DataTextField = "YEAR";
            ddlQueryYear.DataValueField = "YEAR";
            ddlQueryYear.DataBind();
            ddlYear.DataSource=ds.Tables[0].DefaultView;
            ddlYear.DataTextField = "YEAR";
            ddlYear.DataValueField = "YEAR";
            ddlYear.DataBind();


            ds = FSCDLL.DAL.Corpus.GetCorpusMajor();
            ddlQueryMajor.DataSource = ds.Tables[0].DefaultView;
            ddlQueryMajor.DataTextField = "Title";
            ddlQueryMajor.DataValueField = "TitleCN";
            ddlQueryMajor.DataBind();
            ddlQueryMajor.Items[0].Selected=true  ;
            ddlMajor .DataSource = ds.Tables[0].DefaultView;
            ddlMajor.DataTextField = "Title";
            ddlMajor.DataValueField = "TitleCN";
            ddlMajor.DataBind();
            ddlMajor.Items[0].Selected = true;

            Unit width=new Unit( txtKeyWordAc.Width.Value +12)   ;
            ddlQueryYear.Width = width;
            ddlQueryMajor.Width = width;
            ddlQueryJournal.Width = width;

            ddlYear.Width = width;
            ddlMajor.Width = width;
            ddlJournal.Width = width;
            BindJournal(ddlQueryMajor.SelectedValue );
            BindJournal(ddlMajor.SelectedValue);
        }
        /// <summary>
        /// 下拉框联动，根据专业绑定期刊
        /// </summary>
        /// <param name="majorID">专业编号</param>
        private void　BindJournal(string majorID)
        {
            DataSet ds = FSCDLL.DAL.Corpus.GetCorpusJournalByMajor(majorID);
            ddlQueryJournal.DataSource = ds.Tables[0].DefaultView;
            ddlQueryJournal.DataTextField = "Title";
            ddlQueryJournal.DataValueField = "TitleCN";
            ddlQueryJournal.DataBind();
            ddlJournal.DataSource = ds.Tables[0].DefaultView;
            ddlJournal.DataTextField = "Title";
            ddlJournal.DataValueField = "TitleCN";
            ddlJournal.DataBind();
        }
        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitControls()
        {
            string types = "Topic";//Genre Level
            DataSet ds = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);

            //以下为查询的条件部分
            cblQueryTopics.DataSource = ds.Tables[0];
            cblQueryTopics.DataTextField = "Title";
            cblQueryTopics.DataValueField = "ItemID";
            cblQueryTopics.DataBind();

            cblTopics.DataSource = ds.Tables[0].Copy();
            cblTopics.DataTextField = "Title";
            cblTopics.DataValueField = "ItemID";
            cblTopics.DataBind();
            types = "Genre";
            DataSet dsGenre = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);
            cblQueryGenre.DataSource = dsGenre.Tables[0];
            cblQueryGenre.DataTextField = "Title";
            cblQueryGenre.DataValueField = "ItemID";
            cblQueryGenre.DataBind();
            cblGenre.DataSource = dsGenre.Tables[0].Copy();
            cblGenre.DataTextField = "Title";
            cblGenre.DataValueField = "ItemID";
            cblGenre.DataBind();
            types = "Level";
            DataSet dsLevel = FSCDLL.DAL.Corpus.GetCopusExtendByTypes(types);
            cblQueryLevel.DataSource = dsLevel.Tables[0];
            cblQueryLevel.DataTextField = "Title";
            cblQueryLevel.DataValueField = "ItemID";
            cblQueryLevel.DataBind();
            //等级
            cblLevel.DataSource = dsLevel.Tables[0].Copy();
            cblLevel.DataTextField = "Title";
            cblLevel.DataValueField = "ItemID";
            cblLevel.DataBind();
        }
        private void SaveCorpus()
        {//此处要改
            if (txtTitle.Text.Length == 0)
            {
                lbErr.Text = "Title cannot be empty!";
                return;
            }
            if (txtOriginalText.Text.Length == 0)
            {
                lbErr.Text = "Context cannot be empty!";
                return;
            }
            if (ViewState["dtCorpus"] == null)
            {
                ViewState["dtCorpus"] = FSCDLL.DAL.Corpus.GetCorpus().Tables[0];
            }
            DataTable dt = (DataTable)ViewState["dtCorpus"];
            long corpusID = long.Parse(ViewState["Edit"].ToString());
            DataRow dr = dt.NewRow();
            if (corpusID > 0)
                dr = dt.Select("CorpusID=" + corpusID)[0];
            else
            {
                DataRow[] drs = dt.Select("Title='" + txtTitle.Text + "' and OriginalText='" + txtOriginalText.Text + "'");//通过标题和正文判断唯一性
                if (drs.Length > 0)
                {
                    dr = drs[0];
                    corpusID = Convert.ToInt64(dr["CorpusID"].ToString());
                }
                else
                {
                    dr["Created"] = DateTime.Now;
                    dr["Author"] = FSCDLL.Common.Users.UserID;
                    dr["Flag"] = 1;
                }
            }
            dr["Title"] = txtTitle.Text;
            dr["OriginalText"] = txtOriginalText.Text;
            string ids = Common.GetCBListChecked(cblTopics, split);
            dr["TopicID"] = ids;
            ids = Common.GetCBListChecked(cblGenre, split);
            dr["GenreID"] = ids;
            ids = Common.GetCBListChecked(cblLevel, split);
            dr["LevelID"] = ids;
            //赋码未添加
            //dr["CodedText"] ="";
            if (corpusID == 0)
                FSCDLL.DAL.Corpus.InsertCorpus(null, dr,CorpusSource);
            else
            {
                FSCDLL.DAL.Corpus.UpdateCorpus(null, dr);
            }
        }
        #endregion
        #region 查询
        /// <summary>
        /// 构造查询字符串
        /// </summary>
        /// <returns></returns>
        private string GetSelectQuery()
        {
            //此处进行查询，关键词、Level、topic、genre
            string strQuery = "";
            string strSql = "";

            if (CorpusSource == "LC")
            {
                if (cblLevel.SelectedIndex >= 0)
                    strQuery = Common.GetQueryString(cblLevel, "LevelID", split);
                if (cblGenre.SelectedIndex >= 0)
                    strSql = Common.GetQueryString(cblGenre, "GenreID", split);
                if (strQuery == "")
                    strQuery = strSql;
                else
                {
                    if (strQuery.IndexOf("and") > 0)
                        strQuery = strQuery + " and (" + strSql + ")";
                    else
                    {
                        strQuery = "(" + strQuery + ") and (" + strSql + ")";
                    }
                }
                strSql = "";
                if (cblTopics.SelectedIndex >= 0)
                    strSql = Common.GetQueryString(cblTopics, "TopicID", split);
                if (strQuery == "")
                    strQuery = strSql;
                else
                {
                    if (strQuery.IndexOf("and") > 0)
                        strQuery = strQuery + " and (" + strSql + ")";
                    else
                    {
                        strQuery = "(" + strQuery + ") and (" + strSql + ")";
                    }
                }
            }
            else//AC
            {
                strQuery =string.Format( "TopicID='{0}' and GenreID='{1}' and LevelID='{2}'",ddlMajor.SelectedValue,ddlJournal.SelectedValue,ddlYear.SelectedValue );
            }
            //关键词
            if (txtKeyWord.Text.Length > 0)
            {
                strSql = "Title like '%" + txtKeyWord.Text + "%'or Source like '%" + txtKeyWord.Text + "%' or OriginalText like '%" + txtKeyWord.Text + "%' or CodedText like '%" + txtKeyWord.Text + "%'";
                if (strQuery == "")
                    strQuery = strSql;
                else
                {
                    if (strQuery.IndexOf("and") > 0)
                        strQuery = strQuery + " and (" + strSql + ")";
                    else
                    {
                        strQuery = "(" + strQuery + ") and (" + strSql + ")";
                    }
                }
            }
            return strQuery;
        }
        #endregion
    }
}
