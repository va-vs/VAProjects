using System;
using System.Web.Configuration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SmartNEU.DAL;
using System.Data;
using System.DirectoryServices;

namespace SmartNEU.Layouts.SmartNEU
{
    public partial class RetrievePwd : LayoutsPageBase
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            btnSubmit.Click += btnSubmit_Click;
            if (!Page.IsPostBack)
            {
                txtAccount.Text = Request.QueryString["Id"];
                txtAccount_TextChanged(null, null);
            }           
            txtAccount.TextChanged += txtAccount_TextChanged;
        }

        void txtAccount_TextChanged(object sender, EventArgs e)
        {
            string account = txtAccount.Text.Trim();
            if (account.Length == 0) return;
            DirectoryEntry adUser = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                adUser = ADHelper.GetDirectoryEntryByAccount(account);
                string initEmail;
                if (adUser != null)
                {

                    if (adUser.Properties.Contains("mail") && adUser.Properties["mail"].Count > 0)
                    {
                        initEmail = adUser.Properties["mail"][0].ToString();
                        lblMsg.Text = "你在本站注册时所留邮箱为：" + initEmail.Substring(0, 3) + "***" + initEmail.Substring(initEmail.IndexOf("@"));
                    }
                    else
                        lblMsg.Text = "你在本站注册时未提交邮箱地址，请将您的学号（工号）、姓名、校园卡正反面照片发送邮件至smartneu@mail.neu.edu.cn找回密码 ！";
                }
                else
                {
                    lblMsg.Text = "帐号不存在！";
                    txtAccount.Focus();
                }
            });
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            string checkCode = GenerateCheckCode();
            string account = txtAccount.Text.Trim();
            string eMail = txtEmail.Text.Trim();
            //验证用户帐户和邮件是否一致
            DirectoryEntry adUser = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                adUser = ADHelper.GetDirectoryEntryByAccount(account);

                string initEmail;
                if (adUser == null)
                {
                    string message = "帐号不存在";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');document.forms(0).txtAccount.select();</script>");
                    return;
                }
                else
                {
                    if (adUser.Properties.Contains("mail") && adUser.Properties["mail"].Count > 0)
                    {
                        initEmail = adUser.Properties["mail"][0].ToString();
                        if (eMail == initEmail)
                        {
                            DataSet ds = DAL.ResetPassword.GetResetPassword(0);
                            DataRow dr = ds.Tables[0].NewRow();
                            dr["Account"] = account;
                            dr["Email"] = eMail;
                            dr["CheckCode"] = checkCode;
                            dr["StartTime"] = DateTime.Now;
                            dr["IsFinished"] = false;
                            long operateId = DAL.ResetPassword.InsertResetPassword(dr);
                            if (operateId > 0)
                            {
                                SendEmail(operateId, checkCode, eMail);
                            }
                        }
                        else
                        {
                            string message = WebConfigurationManager.AppSettings["resetPwdEmailDifferent"];
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');document.forms(0).txtEmail.select();</script>");
                            txtEmail.Focus();
                            return;
                        }
                    }
                    else
                    {
                        string message = WebConfigurationManager.AppSettings["resetPwdEmailNullAlert"]; // "注册时没有";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');</script>");
                    }

                }
            });
        }
        #endregion
        #region 方法
        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 产生4位验证码
        /// </summary>
        /// <returns></returns>
        private string GenerateCheckCode()
        {
            int number;
            char code;
            string checkCode = String.Empty;

            System.Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                number = random.Next();

                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else
                    code = (char)('A' + (char)(number % 26));

                checkCode += code.ToString();
            }


            return checkCode;
        }
        private void SendEmail(long operateId, string checkCode, string toMail)
        {
            SPSite spSite = SPContext.Current.Site;
            SPWeb spWeb = SPContext.Current.Web;
            string siteName = spWeb.Name.ToString();
            string siteUrl = spWeb.Url.ToString();

            string txtContent = "尊敬的 " + txtAccount.Text.Trim() + " ，您好：<br/><br/>";
            txtContent += "您在 <b>" + siteName + "</b>于" + string.Format("{0:F}", DateTime.Now) + "申请找回密码，重设密码的验证码是：" + checkCode;
            txtContent += "<br/>请你在20分钟内完成找回密码操作。如果你已关闭了操作页面，请<a href='" + siteUrl + "/_layouts/15/SmartNEU/ResetPwd.aspx?sendID=" + operateId.ToString() + "'>点此继续</a>.";
            txtContent += "<br/><br/><br/>如果您没有进行过找回密码的操作，请删除此邮件。";
            txtContent += "<br/><br/>谢谢！";
           
            string email=WebConfigurationManager.AppSettings["emailFrom"];
            if (email != "")
            {
                string[] mails = Common.getEmailFrom(email);
                Common.SendMail(mails[0], mails[1], mails[2], new string[] { toMail }, "忘记密码", txtContent, mails[3]);
                string message = WebConfigurationManager.AppSettings["resetPwdSendEmailAlert"];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');location.href='ResetPwd.aspx?sendID=" + operateId.ToString() + "'</script>");
            }
        }
        #endregion
    }
}

