using System;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.DirectoryServices;
using SmartNEU.DAL;
using System.Web.Configuration;

namespace SmartNEU.Layouts.SmartNEU
{
    public partial class ResetPwd : LayoutsPageBase
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            btnSubmit.Click += btnSubmit_Click;
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["sendID"] != null)
                {
                    long operateID = long.Parse(Request.QueryString["sendID"]);
                    DataSet ds = DAL.ResetPassword.GetResetPassword(operateID);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ViewState["ds"] = ds;
                        txtAccount.Text = dr["Account"].ToString();
                    }
                }
            }
        }
        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ViewState["ds"] == null) return;
            DataRow dr = ((DataSet)ViewState["ds"]).Tables[0].Rows[0];
            DateTime sTime = (DateTime)dr["StartTime"];
            DateTime cTime = DateTime.Now;
            if (!txtCheckCode.Text.Equals(dr["CheckCode"].ToString()))
            {
                string message = "验证码错误，请重新输入";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');</script>");
                txtCheckCode.Focus();
                return;
            }
            if (cTime.Subtract(sTime).Minutes > 20)
            {
                string message = "验证码已过期，请重新执行找回密码操作！";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');top.location.href='" + SPContext.Current.Site.Url + "/_layouts/15/SmartNEU/RetrievePwd.aspx'</script>");
                return;
            }
            dr["EndTime"] = DateTime.Now;
            dr["IsFinished"] = true;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                ADHelper.ChangePassword(txtAccount.Text, txtPwd.Text);
            });
            DAL.ResetPassword.UpdateResetPassword(dr);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('密码重置成功，请重新登录本站！');top.location.href='" + SPContext.Current.Site.Url + "/_layouts/15/Authenticate.aspx'</script>");

        }
        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true;
            }
        }
        #endregion
        #region 方法

        #endregion
    }
}
