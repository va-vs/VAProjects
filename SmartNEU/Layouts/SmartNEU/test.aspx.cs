using System;
using System.Web;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SmartNEU.DAL;
using System.Security.Principal;
using System.DirectoryServices.ActiveDirectory;
using System.Web.Configuration;
using Microsoft.SharePoint.Utilities;
using System.Net;
using System.Net.Mail;
using System.Web.Mail;

namespace SmartNEU.Layouts.SmartNEU
{
    public partial class test : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TestSendEmailNet();
            //testAlert();
        }
        private void testAlert()
        {
            string message = "system.Net.mail<br/>\r\n注册信息已经发送到您的邮件!";
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "')</script>");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer> if (confirm('请核对信息，删除后不能恢复\n    确定删除吗?')) { return true;}else { return false; }</script>",true);
 
           
        }
        private void TestSPUtility()
        {
            try
            {
                bool ok = SPUtility.SendEmail(SPContext.Current.Web, false, false,
    "962204414@qq.com", "E-mail title",
    "E-mail body");
                if (ok)
                {
                    string message = "注册信息已经发送到您的邮件!";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "')</script>");
                }
                else
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('failed')</script>");
            }
            catch (Exception ex)
            {
                Common.ShowMessage(Page, this.GetType(), ex.ToString());
            }
            return;
        }
        private void TestSendEmailNet()
        {
            string email = WebConfigurationManager.AppSettings["emailFrom"];
            if (email != "")
            {
                string[] mails = Common.getEmailFrom(email);
                bool retValue;
                //retValue = Common.SendMail(mails[0].Trim(), mails[1].Trim(), mails[2].Trim(), new string[] { "962204414@qq.com" }, "注册成功", "", mails[3]);
                retValue = SendMail(mails[0].Trim(), mails[1].Trim(), mails[2].Trim(), new string[] { "962204414@qq.com" }, "注册成功", "" );
                if (retValue)
                {
                    string message = "system.Net.mail 注册信息已经发送到您的邮件!";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "')</script>");
                }
                else
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('failed')</script>");
            }
        }
        private void  TestSendEmailWeb()
        {
            //web.mail
            bool ok = SendEmail();
            if (ok)
            {
                string message = "web.mail 注册信息已经发送到您的邮件!";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "')</script>");
            }
            else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('failed')</script>");
            //End web.mail
        }
        private bool SendEmail()
        {
            bool flag = false;
            string sexString = "女士";
            string content = "<div style='font-weight:bold;font-size:16px;line-height:25px;'>亲爱的962204414:<br/>&nbsp;&nbsp;您好,欢迎加入智慧东大。</div><P>您的注册信息如下:<br/>&nbsp;&nbsp;&nbsp;&nbsp; 账  户:  <br>&nbsp;&nbsp;&nbsp;&nbsp; <br/>密  码： <br>&nbsp;&nbsp;请务必保管好您的账户和密码! <br/>如想修改密码,请<a href='http://va.neu.edu.cn/SmartNEU/_layouts/15/ChangePassword/ChangePassword.aspx?Source=/SmartNEU'>点击此处</a>";
            string email = WebConfigurationManager.AppSettings["emailFrom"].Trim ();
            if (email != "")
            {
                string[] mails = Common.getEmailFrom(email);//配置文件信息:邮件地址,邮件密码,邮件smtp服务器地址
                flag = Common.SendWebMail(mails[0], mails[2], mails[3], "962204414@qq.com", "智慧东大 - 账户注册成功", content, "1");
                //flag = Common.SendMailWeb(mails[0].Trim (), mails[2].Trim(),"962204414@qq.com", mails[3], "智慧东大 - 账户注册成功", content);
            }
            return flag;
        }
        /// <summary>
      
        public static bool SendMail(string fromEmail, string fromDisplayName, string pwd, string[] toMail, string toSubject, string toBody)
        {
            ////设置发件人信箱,及显示名字
            MailAddress from = new MailAddress(fromEmail, fromDisplayName);
            //设置收件人信箱,及显示名字 
            //创建一个MailMessage对象
            System.Net.Mail.MailMessage oMail = new System.Net.Mail.MailMessage();
            oMail.From = from;
            for (int i = 0; i < toMail.Length; i++)
            {
                oMail.To.Add(toMail[i].ToString());
            }
            oMail.Subject = toSubject; //邮件标题 
            oMail.Body = toBody; //邮件内容

            oMail.IsBodyHtml = true; //指定邮件格式,支持HTML格式 
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");//邮件采用的编码 

            //发送邮件服务器 +
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.neu.edu.cn";// fromEmail.Substring(fromEmail.IndexOf("@") + 1); //163.com指定邮件服务器"smtp.sina.com";//
           client.Credentials = new NetworkCredential(fromDisplayName, pwd);//指定服务器邮件,及密码
           client.Port = 25;
            try
            {
                client.Send(oMail); //发送邮件
                oMail.Dispose(); //释放资源
                return true;// "恭喜你！邮件发送成功。";
            }
            catch (Exception ex)
            {
                oMail.Dispose(); //释放资源
                return false;// "邮件发送失败，检查网络及信箱是否可用。" + e.Message;
            }


        }
        private void showMsg()
        {
            //if (adUser == null)
            //{
                string message = "帐号不存在";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');document.forms(0).txtAccount.select();</script>");
                return;
            //}
        }
    }
}
