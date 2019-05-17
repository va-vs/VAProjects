using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Web.Mail;
using System.Web.UI;
using System.IO;
using System.DirectoryServices;
using System.Data;

namespace SmartNEU.DAL
{
    public class Common
    {
        #region 验证
        public static bool IsTelephone(string phoneValue)
        {
            string timeExpr = @"^(((\(\d{3}\)|\d{3}-)?\d{8})|((\(\d{4,5}\)|\d{4,5}-)?\d{7,8})|(\d{11}))$";
            Regex rex = new Regex(timeExpr );
            if (rex.IsMatch (phoneValue))
            {
                return true;
            }
            else
                return false;
        }
        public static bool IsEmail(string email)
        {
            string emailExpr = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            if (Regex.IsMatch (email,emailExpr))
                return true;
            else
                return false;
        }
        //只能输入中文
        public static bool IsChinese(string name)
        {
            if (name.Length >0)
            { 
           char[] chars= name.ToCharArray();
            var ret = true;
            for (var i = 0; i <chars.Length ; i++)
                ret = ret && (chars[i]  >= 10000);
            return ret;
            }
            else
            {
                return false;
            }
        }
        //测试工号或学号
        public static  bool IsMatching(string accString)
        {
            bool ismatch = true;
            ismatch = Common.isNumberic(accString);
            if (ismatch)
            {
                if (accString.Length >= 5 && accString.Length <= 8)
                    ismatch = true;
                else
                    ismatch = false;
            }

            return ismatch;
        }
        /// <summary>
        /// 判断输入的字符串是否只包含数字和英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
         public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        public static bool isNumberic(string message)
        {
            //if (message != "" && Regex.IsMatch(message, @"^\d{5}$"))
            Regex rex = new Regex(@"^\d+$");

            if (rex.IsMatch(message))
            {
                return true;
            }
            else
                return false;
        }
        #endregion
        //获取登陆用户的账号， string loginName = currentUser.LoginName;
        public static string GetAccount(string loginName)
        {
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        //用户登录帐号-解析后的
        public static string GetUserDept(string loginName)
        {
            string dept = "";
            DirectoryEntry de = ADHelper.GetDirectoryEntryByAccount(loginName);
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.Filter = ("(SAMAccountName=" + loginName + ")");
            SearchResult dss = ds.FindOne();
            if (dss != null)
            {
                string dpath = dss.Path;
                dpath = dpath.Substring(dpath.IndexOf("OU=") + 3);
                dept = dpath.Substring(0, dpath.IndexOf(","));
            }
            else
                dept = "管理员";
            return dept;
        }
        //用户在ad中是否存在
        public static  bool UserExits( string account)
        {
            DirectoryEntry adUser = ADHelper.GetDirectoryEntryByAccount(account);

            if (adUser != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 用户在AD中是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <param name="userState">返回用户的状态</param>
        /// <returns></returns>
        public static bool UserExits(string account, ref int userState)
        {
            DirectoryEntry adUser = ADHelper.GetDirectoryEntryByAccount(account);
            if (adUser != null)
            {
                try
                {
                    ActiveDs.IADsUser user = (ActiveDs.IADsUser)adUser.NativeObject;

                    userState = user.AccountDisabled ? 0 : 1;
                }
                catch
                {
                    userState = -1;
                }
                return true;
            }

            return false;
        }

        private static void FillUserInfo(ref DataRow dr, string account)
        {
            DirectoryEntry adUser = ADHelper.GetDirectoryEntryByAccount(account);//当前被编辑的用户
            if (adUser != null)
            {
                dr["Name"] = adUser.Properties["displayName"][0];
                //dr["Sex"] = int.Parse(rblSex.SelectedValue);
                if (adUser.Properties.Contains("telephoneNumber"))
                    dr["Telephone"] = adUser.Properties["telephoneNumber"][0];
                if (adUser.Properties.Contains("mail"))
                    dr["Email"] = adUser.Properties["mail"][0];
                dr["Flag"] = 1;
                dr["Modified"] = DateTime.Now;

            }
        }
        //ad中的用户和数据库同步
        public static bool  AddUserFromAD(string account)
        {
            DataSet ds = DAL.User.GetUserByAccount(account);
            if (ds.Tables[0].Rows.Count > 0)//在数据库中找到了当前用户的记录,准备更新操作
            {
                DataRow dr = ds.Tables[0].Rows[0];
                dr["Account"] = account;
                FillUserInfo(ref dr, account);
                DAL.User.UpdateUser(dr);//更新当前用户记录
            }
            else//该用户不存在数据库中,准备添加当前用户为新用户
            {
                DataRow dr = ds.Tables[0].NewRow();
                dr["Account"] = account;
                dr["Created"] = DateTime.Now;
                FillUserInfo(ref dr, account);
                DAL.User.InsertUser(dr);//添加新用户纪录
            }
           
            return true;
        }
        /// <summary>
        /// 信息提示
        /// </summary>
        /// <returns></returns>
        public static bool ShowMessage(System.Web.UI.Page page, Type type, string message)
        {
            page.ClientScript.RegisterStartupScript(type, "message", "<script defer>alert('" + message + "')</script>");
            return true;
        }
        public static string[] getEmailFrom(string emailFrom){
            emailFrom = emailFrom.Replace(" ","");
            string[] mails = emailFrom.Trim().Split(',');
            return mails;
        }
        /// <summary>
        /// 弹出JavaScript小窗口
        /// </summary>
        /// <param name="js">窗口信息</param>
        static public void Alert(string message)
        {
            #region
            string js = @"<Script language='JavaScript'>
                    alert('" + message + "');</Script>";
            HttpContext.Current.Response.Write(js);
            #endregion
        }
        /// <summary>
        /// 打开新的窗口(改成模式窗口)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileName"></param>
        public static void OpenWindow(Page page, string fileName)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "open", "<script defer>window.open('" + fileName + "','_blank');</script>");
            //page.ClientScript.RegisterStartupScript(page.GetType(), "open", "<script defer>window.showModalDialog('" + fileName + "','_blank','dialogWidth=1002px;dialogHeight=600px');</script>");
        }
        public static bool JudgeRight(string RoleID, string CurrRoleID)
        {
            bool isRight = false;
            if (CurrRoleID == RoleID)
            {
                isRight = true;

            }
            return isRight;
        }

        /// <summary>
        /// 记入日志文件
        /// </summary>
        /// <param name="content"></param>
        public static void WriteLog(string content)
        {
            using (StreamWriter write = new StreamWriter("reglog.txt"))
            {
                write.WriteLine(DateTime.Now.ToString());
                write.WriteLine(content);

            }

        }

        /// <summary>
        /// 以字符流的形式下载文件之所以转换成 UTF8 是为了支持中文文件名
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="Response"></param>
        public static void DownLoadFileByStream(string filePath, string fileName, HttpResponse Response)
        {

            FileInfo fi = new FileInfo(filePath);//fullpath指的是文件的物理路径 
            if (!fi.Exists) return;

            FileStream fs = new FileStream(filePath, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            //
            long dataToRead = fs.Length;
            int length;

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            //
            byte[] bytes = new byte[10000];

            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8).Replace("+", "%20"));

            //
            try
            {
                while (dataToRead > 0)
                {
                    if (Response.IsClientConnected)
                    {
                        length = fs.Read(bytes, 0, bytes.Length);
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        bytes = new byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }
                fs.Close();
                Response.End();
            }
            catch
            {

            }
            finally
            {

                if (fs != null)
                {
                    fs.Close();
                }
            }

        }
        #region 外加的方法
        public static bool SendMailWeb(string fromEmail, string fromPwd, string toEmail, string smtpServer, string title, string content)
        {

            try
            {
                System.Web.Mail.MailMessage myMail = new System.Web.Mail.MailMessage();
                myMail.From = fromEmail;
                myMail.To = toEmail;// ;

                myMail.Subject = title + DateTime.Now.ToString();
                myMail.Body = content;
                myMail.BodyFormat = MailFormat.Html;

                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1);
                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", fromEmail); //发送方邮件帐户
                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", fromPwd); //发送方邮件密码
                SmtpMail.SmtpServer = smtpServer;//"smtp." + fromMail.Substring(fromMail.IndexOf("@") + 1);
                SmtpMail.Send(myMail);
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }

        }

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
        public static bool SendMail(string fromEmail, string fromDisplayName, string pwd, string[] toMail, string toSubject, string toBody,string smtp="")
        {
            ////设置发件人信箱,及显示名字
            MailAddress from = new MailAddress(fromEmail, fromDisplayName);
            //设置收件人信箱,及显示名字 
            //MailAddress to = new MailAddress(TextBox1.Text, "");


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
            //oMail.Priority = MailPriority.High;//设置邮件的优先级为高
            //Attachment oAttach = new Attachment("");//上传附件
            //oMail.Attachments.Add(oAttach); 

            //发送邮件服务器 +
            SmtpClient client = new SmtpClient();
            client.Host = smtp;// "smtp.neu.edu.cn";// fromEmail.Substring(fromEmail.IndexOf("@") + 1); //163.com指定邮件服务器smtp.sina.com"smtp.sina.com";//
            //client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential(fromDisplayName, pwd);//指定服务器邮件,及密码
            client.Port = 25;
            
            //发送
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

        /// <summary>
        /// WebMail发邮件
        /// </summary>
        /// <param name="fromMail">发件人邮箱地址</param>
        /// <param name="fromPwd">发件人邮箱密码</param>
        /// <param name="smtpStr">smtp服务器地址</param>
        /// <param name="toMail">收件人地址,支持群发,用";"隔开</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="sendMode">邮件格式:0是纯文本,1是html格式化文本</param>
        /// <returns></returns>
        public static bool SendWebMail(string fromMail,string fromPwd, string smtpStr, string toMail,  string subject, string body, string sendMode)
        {
            try
            {
                System.Web.Mail.MailMessage myMail = new System.Web.Mail.MailMessage();
                myMail.From = fromMail;
                myMail.To = toMail;
                //myMail.Cc = ccMail;string ccMail, string bccMail,//抄送和密送
                //myMail.Bcc = bccMail;
                myMail.Subject = subject;
                myMail.Body = body;
                myMail.BodyFormat = sendMode == "0" ? MailFormat.Text : MailFormat.Html;
                //附件
                //string ServerFileName = "";
                //if (this.upfile.PostedFile.ContentLength != 0)
                //{
                //    string upFileName = this.upfile.PostedFile.FileName;
                //    string[] strTemp = upFileName.Split('.');
                //    string upFileExp = strTemp[strTemp.Length - 1].ToString();
                //    ServerFileName = Server.MapPath(DateTime.Now.ToString("yyyyMMddhhmmss") + "." + upFileExp);
                //    this.upfile.PostedFile.SaveAs(ServerFileName);
                //    myMail.Attachments.Add(new MailAttachment(ServerFileName));
                //} 
                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1);
                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", fromMail); //发送方邮件帐户
                myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", fromPwd); //发送方邮件密码
                SmtpMail.SmtpServer = smtpStr ;//"smtp." + fromMail.Substring(fromMail.IndexOf("@") + 1);
                SmtpMail.Send(myMail);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

    }

}
