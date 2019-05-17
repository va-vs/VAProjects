using System;
using Microsoft.SharePoint;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Util;
using System.Web.Mail;
using Microsoft.SharePoint.WebControls;
using System.DirectoryServices;

namespace SmartNEU.Layouts.SmartNEU
{
    public partial class MailTest : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)

            {

                format.Items.Add(new ListItem("文本", "0"));

                format.Items.Add(new ListItem("HTML", "1"));

                format.Items[0].Selected = true;

                fromMail.Text = "smartneu@mail.neu.edu.cn"; //发送方邮件

                fromMail.Enabled = false;

            }

        }
        private bool SendMail(string fromMail, string toMail, string ccMail, string bccMail, string subject, string body, string sendMode)

        {

            try

            {
                MailMessage myMail = new MailMessage();

                myMail.From = fromMail;

                myMail.To = toMail;

                myMail.Cc = ccMail;

                myMail.Bcc = bccMail;

                myMail.Subject = subject;

                myMail.Body = body;

                myMail.BodyFormat = sendMode == "0" ? MailFormat.Text : MailFormat.Html;

                //附件

                string ServerFileName = "";

                if (this.upfile.PostedFile.ContentLength != 0)

                {

                    string upFileName = this.upfile.PostedFile.FileName;

                    string[] strTemp = upFileName.Split('.');

                    string upFileExp = strTemp[strTemp.Length - 1].ToString();

                    ServerFileName = Server.MapPath(DateTime.Now.ToString("yyyyMMddhhmmss") + "." + upFileExp);

                    this.upfile.PostedFile.SaveAs(ServerFileName);

                    myMail.Attachments.Add(new MailAttachment(ServerFileName));

                }



                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1);

                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", "smartneu@mail.neu.edu.cn"); //发送方邮件帐户

                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", "ccc.neu.edu.cn"); //发送方邮件密码

                SmtpMail.SmtpServer = "smtp.neu.edu.cn";//"smtp." + fromMail.Substring(fromMail.IndexOf("@") + 1);
                
                SmtpMail.Send(myMail);

                return true;

            }

            catch

            {

                return false;

            }

        }

        protected void send_Click(object obj, EventArgs e)

        {
            string content = "<div style='font-weight:bold;font-size:16px;line-height:25px;'>亲爱的测试员先生 :<br/>&nbsp;&nbsp;您好,欢迎加入智慧东大网站。</div><P>&nbsp;&nbsp;&nbsp;&nbsp;您的注册信息如下:<br>&nbsp;&nbsp;&nbsp;&nbsp;" + "密  码：123456" + "<br>&nbsp;&nbsp;&nbsp;&nbsp;" + "账  户: " + "ccc\\ceshiyuan" + "<br>&nbsp;&nbsp;请务必保管好您的账户和密码,如想修改密码,请<a href='http://va.neu.edu.cn/SmartNEU/_layouts/15/ChangePassword/ChangePassword.aspx?Source=/SmartNEU'>点击此处</a>";
            bool flag = SendMail(fromMail.Text, toMail.Text, ccMail.Text, bccMail.Text, subject.Text, content, format.SelectedValue);
            //bool flag = Common.SendMail("smartneu@mail.neu.edu.cn","ccc.neu.edu.cn","smtp.neu.edu.cn", new string[] { toMail.Text }, subject.Text, content);
            //Common.SendMail(mails[0], mails[1], mails[2], new string[] { txtEmail.Text.Trim() }, "注册成功", content);//配置文件信息:邮件地址,邮件密码,邮件smtp服务器地址
            if (flag == true)

            {

                Response.Write("<script>alert('发送成功!');</script>");

            }

            else

            {

                Response.Write("<script>alert('发送失败!');</script>");

            }

        }

        //批量查询Ad用户
        protected void btnGetUsers_Click(object sender, EventArgs e)
        {
            var dirEntry = new DirectoryEntry(string.Format("LDAP://{0}/{1}", "ccc.neu.edu.cn", "DC=ccc,DC=neu,DC=edu,DC=cn"));
            var searcher = new DirectorySearcher(dirEntry)
            {
                Filter = "(&(&(objectClass=user)(objectClass=person)))"
            };
            var resultCollection = searcher.FindAll();
        }
        public SearchResultCollection filterUsers(DateTime dateFilter)
        {
            //GetCompList(DateTime.Now.AddDays(-1)); //This sets the filter to one day previous

            //Declare new DirectoryEntry and DirectorySearcher
            DirectoryEntry domainRoot = new DirectoryEntry(string.Format("LDAP://{0}/{1}", "ccc.neu.edu.cn", "DC=ccc,DC=neu,DC=edu,DC=cn"));
            string rootOfDomain = domainRoot.Properties["rootDomainNamingContext"].Value.ToString();
            DirectorySearcher dsSearch = new DirectorySearcher(rootOfDomain);

            //Set the properties of the DirectorySearcher
            dsSearch.Filter = "(&(&(objectClass=user)(objectClass=person)(whenCreated>=" + dateFilter.ToString("yyyyMMddHHmmss.sZ") + ")))";
            dsSearch.PageSize = 2000;
            dsSearch.PropertiesToLoad.Add("distinguishedName");
            dsSearch.PropertiesToLoad.Add("whenCreated");
            dsSearch.PropertiesToLoad.Add("description");
            dsSearch.PropertiesToLoad.Add("operatingSystem");
            dsSearch.PropertiesToLoad.Add("name");
            
            //Execute the search
            SearchResultCollection usersFound = dsSearch.FindAll();
            return usersFound;
        }
        


    }
}
