using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkEvaluate.Layouts.WorkEvaluate
{
    public partial class ScoreStandard : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                LoadData();
            ViewState["dsStandard"] = null;
            ShowStandard();
            ddlQiCi.SelectedIndexChanged += ddlQiCi_SelectedIndexChanged;
            ddlWorksType.SelectedIndexChanged += ddlQiCi_SelectedIndexChanged;
            btnSave .Click +=btnSave_Click;
        }

        void ddlQiCi_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["dsStandardByType"] = null;
            ViewState["dsStandard"] = null;
            ShowStandard();
        }
        #region 初始控件及方法

        protected Button btnSave;
        protected HtmlGenericControl divEditContent;
        protected DropDownList ddlQiCi;
        protected DropDownList ddlWorksType;
        private void LoadData()
        {
            DataSet ds = DAL.Periods.GetPeriodByCourseID(BLL.Course.GetCourseID());
            DataSet dsTmp = ds.Clone();
            DataRow dr = dsTmp.Tables[0].NewRow();
            dr["PeriodTitle"] = "";
            dr["PeriodID"] = 0;
            dsTmp.Tables[0].Rows.Add(dr);
            dsTmp.Merge(ds);
            ddlQiCi.DataSource = dsTmp.Copy();
            ddlQiCi.DataTextField = "PeriodTitle";
            ddlQiCi.DataValueField = "PeriodID";
            ddlQiCi.DataBind();
            if (dsTmp.Tables[0].Rows.Count > 1)
                ddlQiCi.SelectedIndex = 1;
            ds = BLL.WorksType.GetWorksTypeTopLevel();
            ddlWorksType.DataSource = ds.Copy();
            ddlWorksType.DataTextField = "WorksTypeName";
            ddlWorksType.DataValueField = "WorksTypeID";
            ddlWorksType.DataBind();

        }
        private void ShowStandard()
        {
            DataSet dsScoreStandard = GetPeriodStandarad;
            ShowEditContent(dsScoreStandard );
        }
        private void InitControlDetail(int i,DataRow drStandard ,string score,string des,ref Table tbl)
        {
            TableRow tRow;
            TableCell tCell;
            Label  lstStandard;
            TextBox txtContent;
            tRow = new TableRow();
            tCell = new TableCell();
            lstStandard = new Label();
            lstStandard.Width = 100;
            lstStandard.ID = "lst" + i.ToString();
            lstStandard.Text = drStandard["StandardName"].ToString() ;
            tCell.Controls.Add(lstStandard);

            tRow.Cells.Add(tCell);
            txtContent = new TextBox();
            txtContent.ID = "txtScore" + i.ToString() + "_" + drStandard["StandardID"].ToString ();
            txtContent.Width = 20;
            if (score != "")
                txtContent.Text = score;
            tCell = new TableCell();
            tCell.Controls.Add(txtContent);
            tCell.Controls.Add( new LiteralControl ("分（设置指标分数）"));

            tRow.Cells.Add(tCell);
            tbl.Rows.Add(tRow);

            lstStandard = new Label();
            lstStandard.Width = 400;
            lstStandard.ID = "des" + i.ToString();
            lstStandard.Font.Size = 8;
            lstStandard.ForeColor = System.Drawing.Color.Gray;
            lstStandard.Text = drStandard["Description"].ToString() ;
            tRow = new TableRow();
            tCell = new TableCell();
            tCell.ColumnSpan = 2;
            tCell.Controls.Add(lstStandard);
            tRow.Cells.Add(tCell);
            tbl.Rows.Add(tRow);

            txtContent = new TextBox();
            txtContent.ID = "txtDes"+i.ToString ();
            txtContent.TextMode = TextBoxMode.MultiLine;
            txtContent.Rows = 5;
            txtContent.Width = 400;
            if (des != "")
                txtContent.Text = des;
            else
                txtContent.Text = "分条描述指标的具体评分标准";
            tRow = new TableRow();
            tCell = new TableCell();
            tCell.ColumnSpan = 2;
            tCell.Controls.Add(txtContent);
            tRow.Cells.Add(tCell);
            tbl.Rows.Add(tRow);
        }
        private void ShowEditContent(DataSet dsScoreStandard)
        {
            divEditContent.Controls.Clear();
            int i = 1;
            Table tbl = new Table();
            tbl.CellSpacing = 2;
            if (dsScoreStandard.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsScoreStandard.Tables[0].Rows)
                {
                    InitControlDetail(i, dr, dr["Score"].ToString(), dr["StandardDescription"].ToString(), ref tbl);
                    i += 1;
                }
            }
            else
            {
                DataSet dsStandardByType = GetScoreStandardByType;
                if (dsStandardByType.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsStandardByType.Tables[0].Rows)
                    {
                        InitControlDetail(i, dr, dr["Score"].ToString(), dr["StandardDescription"].ToString(), ref tbl);
                        i += 1;
                    }
                }
                else
                {
                    DataSet dsStandard = DSAllStandard;
                    i = 1;
                    foreach (DataRow dr in dsStandard.Tables[0].Rows)
                    {
                        InitControlDetail(i, dr, "", "", ref tbl);
                        i += 1;
                    }
                }
            }
            //TableRow tRow = new TableRow();
            //TableCell tCell = new TableCell();
            //tCell.ColumnSpan = 2;
            //tCell.HorizontalAlign = HorizontalAlign.Center;
            //Button btn = new Button();
            //btn.Text = "保 存";
            //btn.Click += btn_Click;
            //tCell.Controls.Add(btn);
            //tRow.Cells.Add(tCell);
            //tbl.Rows.Add(tRow);
            divEditContent.Controls.Add(tbl);
        }
        /// <summary>
        /// 五个指标共90分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSave_Click(object sender, EventArgs e)
        {
            Table tbl = (Table)divEditContent.Controls[0];
            DataSet ds = GetPeriodStandarad;
            DataSet dsTmp = ds.Clone();
            string txtScore;
            string lstValue;
            string txtDes;
            DataRow[] drs;
            if (PeriodID == 0)
                ds = GetScoreStandardByType;
            DataRow dr;
            int result = 0;
            int i = 0;
            while (i < DSAllStandard.Tables[0].Rows.Count * 3)
            {
                lstValue = ((TextBox)tbl.Rows[i].Cells[1].Controls[0]).ID;
                lstValue = lstValue.Substring(lstValue.IndexOf("_") + 1);
                txtScore = ((TextBox)tbl.Rows[i].Cells[1].Controls[0]).Text;
                txtDes = ((TextBox)tbl.Rows[i + 2].Cells[0].Controls[0]).Text;
                if (txtScore == "")
                    txtScore = "0";
                if (PeriodID > 0)
                {
                    drs = ds.Tables[0].Select("StandardID=" + lstValue);
                    if (drs.Length > 0)
                    {
                        drs[0]["StandardDescription"] = txtDes.Trim();
                        drs[0]["Score"] = txtScore.Trim();
                        drs[0]["Modified"] = DateTime.Now;
                        drs[0]["ModifiedBy"] = DAL.Common.LoginID;
                        DAL.Standard.UpdatePeriodStandard(drs[0]);
                        result += 1;
                    }
                    else
                    {
                        if (txtScore != "")
                        {
                            dr = dsTmp.Tables[0].NewRow();
                            dr["PeriodID"] = PeriodID;
                            dr["WorkTypeID"] = WorksTypeID;
                            dr["StandardID"] = int.Parse(lstValue);
                            dr["StandardDescription"] = txtDes.Trim();
                            dr["Score"] = int.Parse(txtScore.Trim());
                            dr["Created"] = DateTime.Now;
                            dr["CreatedBy"] = DAL.Common.LoginID;
                            dr["Flag"] = 1;
                            DAL.Standard.InsertPeriodStandard(dr);
                            result += 1;
                        }
                    }
                }
                else
                {

                    drs = ds.Tables[0].Select("StandardID=" + lstValue);
                    if (drs.Length > 0)
                    {

                        drs[0]["Score"] = int.Parse(txtScore.Trim());
                        drs[0]["StandardDescription"] = txtDes.Trim();
                        DAL.WorksType.UpdateWorksTypeScoreStandard(drs[0]);
                        result += 1;
                    }
                    else
                    {
                        dr = ds.Tables[0].NewRow();
                        dr["WorkTypeID"] = WorksTypeID;
                        dr["StandardID"] = lstValue;
                        dr["StandardDescription"] = txtDes.Trim();
                        dr["Score"] = int.Parse(txtScore.Trim());
                        dr["Flag"] = 1;
                        DAL.WorksType.InsertWorksTypeScoreStandard(dr);
                        result += 1;
                    }
                }
                //保存或更新
                i += 3;
            }
            if (result > 0)
            {
                ViewState["dsStandard"] = null;
                DAL.Common.ShowMessage(this.Page, this.GetType(), "保存成功");

            }
            else
                DAL.Common.ShowMessage(this.Page, this.GetType(), "分数不能空");

        }
        /// <summary>
        /// 指标评分
        /// </summary>
        /// <param name="dr"></param>
        #endregion
        #region 属性
        private  long periodID;
        private  int workTypeID;
        private long PeriodID
        {
            get
            {
                periodID = long.Parse (ddlQiCi.SelectedValue);
                return periodID;
            }
        }
        private int WorksTypeID
        {
            get
            {
                workTypeID = int.Parse(ddlWorksType.SelectedValue);
                return workTypeID;
            }
        }
        private DataSet GetScoreStandardByType
        {
            get
            {
                if (ViewState["dsStandardByType"] == null)
                {
                    DataSet ds = DAL.WorksType.GetWorksTypeScoreStandardByTypeID(WorksTypeID);
                    ViewState["dsStandardByType"] = ds;
                }
                return (DataSet)ViewState["dsStandardByType"]; 
            }
        }
        private DataSet GetPeriodStandarad
        {
            get
            {
                if (ViewState["dsStandard"] == null)
                {
                    DataSet ds = DAL.Works.GetScoreStandardByWorksType(WorksTypeID,PeriodID);
                    ViewState["dsStandard"] = ds;
                }
                return (DataSet)ViewState["dsStandard"];
            }
        }
        //获取所有的指标,所有类别共用一个指标，分值和描述不同。
        private DataSet DSAllStandard
        {
            get
            {
                if (ViewState["dsSublevel"] == null)
                {
                    DataSet ds = DAL.Works.GetScoreStandardSubLevel();
                    ViewState["dsSublevel"] = ds;
                }
                return (DataSet)ViewState["dsSublevel"];
            }
        }
        #endregion
    }
}
