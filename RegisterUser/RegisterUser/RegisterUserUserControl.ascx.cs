using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Runtime.InteropServices;
using System.Collections;

using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Security.Principal;
using System.DirectoryServices.ActiveDirectory;

using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.IO;

namespace RegisterUser.RegisterUser
{
    public partial class RegisterUserUserControl : UserControl
    {
        #region event
        public RegisterUser WebPartObj { get; set; }
           
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDepartOfTeacher();

            }
            btnSave.Click += btnSave_Click;
            txtPwd.Attributes["value"] = txtPwd.Text;
            txtPwd1.Attributes["value"] = txtPwd1.Text;
        }
        void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string account = txtAccount.Text.Trim().Replace(" ", "");
              
                    bool succeed = SaveAD(account, txtName.Text.Trim(), txtEmail.Text.Trim(), txtTelephone.Text.Trim(), txtPwd.Text.Trim(), rblRole.SelectedValue, ddlSchool.SelectedItem == null ? "其它" : ddlSchool.SelectedItem.Text, false );
                    if (succeed)
                    {
                      
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('注册成功,请等待审核！');top.location.href='" + SPContext.Current.Site.Url + "'</script>");
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>LoginSys('" + ADHelper.Domain + "','" + account + "','" + txtPwd.Text.Trim() + "');</script>");

                    }
                    else
                    {
                        Common.ShowMessage(Page, this.GetType(), "用户已经存在！");
                    }
                }
            catch (Exception ex)
            {
                Common.ShowMessage(Page, this.GetType(), "注册失败！" + ex.ToString());
            }
        }
        #endregion
        #region 方法
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        WindowsImpersonationContext impersonationContext;

        [DllImport("advapi32.dll")]
        public static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
        private bool impersonateValidUser(String userName, String domain, String password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                    LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                CloseHandle(tokenDuplicate);
            return false;
        }

        private void undoImpersonation()
        {
            impersonationContext.Undo();
        }
        private bool SaveAD(string userAccount,string txtName,string txtEmail,string txtTelephone ,string txtPwd, string roleName,string schoolName, bool userEnabled)
        {
            bool retValue = false;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    string domain = ADHelper.Domain;
                    string strConst = HiddenField1.Value;
                    if (impersonateValidUser("administrator", domain, strConst.Substring(strConst.IndexOf(" ") + 1)))
                    {
                        string ouName = ddlSchool.SelectedItem.Text;// "iSmart";// System.Configuration.ConfigurationManager.AppSettings["adPath"];
                        if (rblRole.SelectedIndex == 0)
                            ouName = ADHelper.GetDirectoryEntryOfOU(WebPartObj.TeacherPath, ouName);
                        else
                            ouName = ADHelper.GetDirectoryEntryOfOU(WebPartObj.StudentPath, ouName);
                        retValue = ADHelper.AddUser(userAccount, txtName, txtEmail, txtTelephone, txtPwd, ouName, ddlSchool.SelectedItem.Text , schoolName, userEnabled);
                        undoImpersonation();
                    }
                    else
                    {
                        //Your impersonation failed. Therefore, include a fail-safe mechanism here.
                    }

                });
                return retValue;
            }
            catch
            {
                return false;
            }
        }
        private void SendEmail()
        {
            string content = "您注册的用户名：" + this.txtAccount.Text.Trim() + "<br>" + "您注册的密码：" + this.txtPwd.Text.Trim() + "<br>" + "登录用户名：  <FONT size=5>" + ADHelper.Domain + "\\" + txtAccount.Text.Trim() + "</FONT>";
            
            Common.SendMail("training@mail.neu.edu.cn", "training", "110004cc", new string[] { txtEmail.Text.Trim() }, "注册成功", content);
            string message = "注册信息已经发送到您的邮件，24小时后您的账号将激活，注意查收";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');location.href='" + SPContext.Current.Web.Url + "'</script>");

        }
        /// <summary>
        /// 生成验证码的随机数
        /// </summary>
        /// <returns>返回五位随机数</returns>
        private string GenerateCheckCode()
        {
            int number;
            char code;
            string checkCode = String.Empty;

            Random random = new Random();

            for (int i = 0; i < 5; i++)//可以任意设定生成验证码的位数
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
        #endregion
        #region 事件及方法
        //为注册的教师添加系部
        private void FillDepartOfTeacher()
        {
            ddlSchool.Items.Clear();
          
            if (rblRole.SelectedIndex == 0)
            {
                ddlSchool.Items.Add(WebPartObj.ADPath);
                SPSecurity.RunWithElevatedPrivileges(delegate()
               {
                   ArrayList depts = ADHelper.GetDeptsOfOU(WebPartObj.TeacherPath, WebPartObj.ADPath);
                   if (depts.Contains(WebPartObj.ADPath))
                       depts.Remove(WebPartObj.ADPath);
                   foreach (string dept in depts)
                       ddlSchool.Items.Add(dept);
               });

            }
            else
            {

            }
        }
        //教师和学生注册的位置不一样,默认为教师
        protected void rblRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        #endregion
    }
    public class Common
    {
public static bool ShowMessage(System.Web.UI.Page page, Type type, string message)
        {
            page.ClientScript.RegisterStartupScript(type, "message", "<script defer>alert('" + message + "')</script>");
            return true;
        }
        #region 外加的方法
        /// <summary>
        /// 以指定的邮箱向多个用户发送邮件
        /// </summary>
        /// <param name="fromEmail">发送邮件的源</param>
        /// <param name="fromDisplayName">显示名称</param>
        /// <param name="pwd">发送源的邮箱密码</param>
        /// <param name="toMail">发送的目标邮箱</param>
        /// <param name="toSubject">发送的主题</param>
        /// <param name="toBody">发送的内容</param>
        /// <returns></returns>
        public static bool SendMail(string fromEmail, string fromDisplayName, string pwd, string[] toMail, string toSubject, string toBody)
        {
            ////设置发件人信箱,及显示名字
            MailAddress from = new MailAddress(fromEmail, fromDisplayName);
            //设置收件人信箱,及显示名字 
            //MailAddress to = new MailAddress(TextBox1.Text, "");


            //创建一个MailMessage对象
            MailMessage oMail = new MailMessage();

            oMail.From = from;
            for (int i = 0; i < toMail.Length; i++)
            {
                oMail.To.Add(toMail[i].ToString());
            }


            oMail.Subject = toSubject; //邮件标题 
            oMail.Body = toBody; //邮件内容

            oMail.IsBodyHtml = true; //指定邮件格式,支持HTML格式 
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");//邮件采用的编码 
            //oMail.Priority = MailPriority.High;//设置邮件的优先级为高
            //Attachment oAttach = new Attachment("");//上传附件
            //oMail.Attachments.Add(oAttach); 

            //发送邮件服务器 +
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.neu.edu.cn";// fromEmail.Substring(fromEmail.IndexOf("@") + 1); //163.com指定邮件服务器smtp.sina.com
            client.Credentials = new NetworkCredential(fromEmail, pwd);//指定服务器邮件,及密码

            //发送
            try
            {
                client.Send(oMail); //发送邮件
                oMail.Dispose(); //释放资源
                return true;// "恭喜你！邮件发送成功。";
            }
            catch
            {
                oMail.Dispose(); //释放资源
                return false;// "邮件发送失败，检查网络及信箱是否可用。" + e.Message;
            }


        }
        #endregion

   }
}
