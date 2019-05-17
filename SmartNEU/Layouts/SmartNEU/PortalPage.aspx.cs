using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SmartNEU.DAL;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Linq;

namespace SmartNEU.Layouts.SmartNEU
{
    public partial class PortalPage : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnCheck.Click += btnCheck_Click;
        }

        void btnCheck_Click(object sender, EventArgs e)
        {
            string acc = IDTxt.Text.ToString();
            acc=acc.Trim();//去空格
            if (isIDNum(acc))
            {
                Common.ShowMessage(Page, this.GetType(), "您尚未输入学号或工号或格式不正确, 请确认后再试!");
            }
            else
            {
                bool isAcc = false;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    isAcc = Common.UserExits(acc);
                });                    
                HadAcc.Visible = isAcc;
                NotHadAcc.Visible = !isAcc;
            }               
        }


        protected bool isIDNum(string id)
        {
            bool isID = false;
            if (id != "" && Regex.IsMatch(id, @"^\d{5,8}$"))
            {
                //成功
                isID= true;
            }
            return isID;                
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailinfo">邮件的一些属性</param>
        /// <param name="smtpname">SMTP事务的主机名称</param>
        /// <param name="username">自己的邮箱登录名</param>
        /// <param name="pwd">邮箱的密码</param>
        /// <returns></returns>
        /// <author>Wilhelm Von Arminius</author>
        public Boolean SendEmail(MailMessage mailinfo, string username,string smtpStr, string adminusername, string adminpwd)
        {
            bool flag = false;
            try
            {
                MailAddress from = new MailAddress(adminusername); //邮件的发件人
                MailMessage mail = new MailMessage();
                //设置邮件的标题
                mail.Subject = "淘宝会员密码";
                //设置邮件的发件人
                mail.From = from;
                //设置邮件的收件人
                string address = username;
                string displayName = username.Split('@').FirstOrDefault().ToString();
                mail.To.Add(new MailAddress(address, displayName, System.Text.Encoding.UTF8));

                /**/
                //设置邮件的内容
                mail.Body = mailinfo.Body;
                //设置邮件的格式 
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;

                //设置邮件的发送级别
                mail.Priority = MailPriority.Normal;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

                SmtpClient client = new SmtpClient();
                //设置用于 SMTP 事务的主机的名称，填IP地址也可以了            
                client = new System.Net.Mail.SmtpClient(smtpStr); //其它的，列如：smtp.qq.com,smtp.sina.com

                //设置用于 SMTP 事务的端口，默认的是 25
                client.Port = 25;
                client.UseDefaultCredentials = true;
                string ss = adminusername.Split('@').FirstOrDefault().ToString();
                // client.Credentials = new System.Net.NetworkCredential(ss, adminpwd); //这一行代码会报错，不能去掉@之后语句.
                client.Credentials = new System.Net.NetworkCredential(adminusername, adminpwd); //("bianbill@126.com","111") 
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 300000;
                client.Send(mail);
                flag = true;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return flag;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            MailMessage mailinfo = new MailMessage();
            mailinfo.Body = mailbody.Text;
            string username =gettxt.Text;
            string smtpStr = smtptxt.Text;
            string adminusername =sendtxt.Text;
            string adminpwd = pwdtxt.Text;
            SendEmail(mailinfo, username, smtpStr, adminusername, adminpwd);
        }
    }
}
