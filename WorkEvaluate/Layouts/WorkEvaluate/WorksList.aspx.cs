using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace WorkEvaluate.Layouts.WorkEvaluate
{
    public partial class WorksList : LayoutsPageBase
    {
        #region 属性
        /// <summary>
        /// 1、评价训练 2、评分 3、公示点评
        /// </summary>
        private int PeriodTime
        {
            get
            {
                if (ViewState["periodTime"] == null)
                {
                    ViewState["periodTime"] = BLL.Course.JudgeDate(PeriodID);
                }
                return (int)ViewState["periodTime"];
            }
        }
        private long PeriodID
        {
            get
            {
                try
                {
                    return long.Parse(ddlQiCi.SelectedValue);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 当前期次的待评作品
        /// </summary>
        private DataSet GetWorksByPeriod
        {
            get
            {
                if (ViewState["WorksAll"] == null)
                {

                    DataSet dsSearch = DAL.Works.GetWorksByPeriodID(  PeriodID);
                    ViewState["WorksAll"] = dsSearch;
                }
                return (DataSet)ViewState["WorksAll"];
            }
        }
        #endregion
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            gvWorks.PageIndexChanging += new GridViewPageEventHandler(gvWorks_PageIndexChanging);
            gvWorks.RowCommand += new GridViewCommandEventHandler(gvWorks_RowCommand);
            gvWorks.RowDataBound += gvWorks_RowDataBound;
            ddlQiCi.SelectedIndexChanged += ddlQiCi_SelectedIndexChanged;
            if (!Page.IsPostBack)
            {
                LoadData();
                if (PeriodTime < 3)
                {
                    //Response.Write(DAL.Common.SPWeb.Url);
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "message", "alert('作品未到公示期');document.location.replace('" + DAL.Common.SPWeb.Url + "');", true);
                    return;
                }

                gvWorks.DataKeyNames = new string[] { "WorksID" };
                gvWorks.PageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["pageSize"]);

                btnSearch_Click(null, null);
            }

        }

        void gvWorks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {

                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[i].ForeColor = ColorTranslator.FromHtml("#000000");
                }
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //模板列
                ((HyperLink)e.Row.Cells[4].FindControl("lnkScore")).NavigateUrl = "Comments.aspx?WorksID=" + e.Row.Cells[0].Text + "&&PeriodID=" + PeriodID;
            }

        }

        void ddlQiCi_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch_Click(null, null);

        }

        void gvWorks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetail" || e.CommandName == "WorksScore")
            {
                GridViewRow drv = ((GridViewRow)(((LinkButton)(e.CommandSource)).Parent.Parent)); //此得出的值是表示那行被选中的索引值 
                string worksID = gvWorks.DataKeys[drv.RowIndex].Value.ToString();
                //Response.Redirect ("Comments.aspx?WorksID=" + worksID + "&&PeriodID=" + PeriodID,false );
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>window.open('Comments.aspx?WorksID=" + worksID + "&&PeriodID="+PeriodID +"')</script>");
                //LinkButton lnk = (LinkButton)(e.CommandSource);
                //    lnk.PostBackUrl = "Comments.aspx?WorksID=" + worksID + "&&PeriodID=" + PeriodID;

            }
        }
        void gvWorks_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvWorks.PageIndex = e.NewPageIndex;
            DataTable dt = ((DataSet)ViewState["dsSearch"]).Tables[0];
            GridBind(dt.DefaultView);

        }
        void btnSearch_Click(object sender, EventArgs e)
        {
            DataSet dsSearch = GetWorksByPeriod;
            ViewState["dsSearch"] = dsSearch;
            DataView dv = new DataView(dsSearch.Tables[0]);
            GridBind(dv);
        }
        #endregion
        #region 方法

        private void LoadData()
        {
            DataSet ds = DAL.Periods.GetPeriodByCourseID(BLL.Course.GetCourseID());
            ddlQiCi.DataSource = ds;
            ddlQiCi.DataTextField = "PeriodTitle";
            ddlQiCi.DataValueField = "PeriodID";
            ddlQiCi.DataBind();
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void GridBind(DataView dv)
        {
            gvWorks.DataSource = dv;
            gvWorks.DataBind();
        }
        #endregion
    }
}
