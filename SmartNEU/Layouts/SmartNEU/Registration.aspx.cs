using System;
using System.Web;
using System.Web.Mail;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SmartNEU.DAL;
using System.Security.Principal;
using System.DirectoryServices.ActiveDirectory;
using System.Web.Configuration;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace SmartNEU.Layouts.SmartNEU
{
    public partial class Registration : LayoutsPageBase
    {
        #region 属性
        private string CurrentIP
        {
            get
            {
                // look for current selected user in ViewState
                object currentIP = this.ViewState["_CurrentIP"];
                if (currentIP == null)
                {
                    HttpRequest request = HttpContext.Current.Request;
                    string result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(result))
                    {
                        result = request.ServerVariables["REMOTE_ADDR"];
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        result = request.UserHostAddress;
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        result = "0.0.0.0";
                    }
                    this.ViewState["_CurrentIP"] = result;
                    return result;
                }
                else
                    return (string)currentIP;
            }

            set
            {
                this.ViewState["_CurrentIP"] = value;
            }
        }
        #endregion
        #region 事件
        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                InitControl();
            }
            btnSave.Click += btnSave_Click;
            btnClose.Click += BtnClose_Click;
            txtAccount.TextChanged += txtAccount_TextChanged;
            txtPwd.TextChanged += txtPwd_TextChanged;
            txtPwd1.TextChanged += txtPwd1_TextChanged;
            txtName.TextChanged += txtName_TextChanged;
            txtTelephone.TextChanged += txtTelephone_TextChanged;
            txtEmail.TextChanged += txtEmail_TextChanged;
            //ddlProvince.SelectedIndexChanged += ddlProvince_SelectedIndexChanged;
            txtPwd.Attributes["value"] = txtPwd.Text;
            txtPwd1.Attributes["value"] = txtPwd1.Text;
        }

        void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (!Common.IsEmail(txtEmail.Text))
                lblEmailMsg.Text = "E-mail地址格式错误!";
            else
                lblEmailMsg.Text = "";
        }

        void txtTelephone_TextChanged(object sender, EventArgs e)
        {
            if (!Common.IsTelephone(txtTelephone.Text))
                lblTelMsg.Text = "电话输入格式错误";
            else
                lblTelMsg.Text = "";
        }

        void txtName_TextChanged(object sender, EventArgs e)
        {
            if (!Common.IsChinese(txtName.Text))
                lblNameMsg.Text = "姓名只能输入中文";
            else
                lblNameMsg.Text = "";
        }

        void txtPwd1_TextChanged(object sender, EventArgs e)
        {
            if (txtPwd.Text.Length !=txtPwd1.Text.Length )
                lblPwd1Msg.Text = "密码与确认密码不一致！";
            else
                lblPwd1Msg.Text = "";
        }

        void txtPwd_TextChanged(object sender, EventArgs e)
        {
            if (txtPwd.Text.Length < 6)
                lblPwdMsg.Text = "密码长度不能小于6！";
            else
                lblPwdMsg.Text = "";
        }
        //账号格式是否正确；是否已存在：若存在，则弹出登陆页面
        void txtAccount_TextChanged(object sender, EventArgs e)
        {
            string accString = txtAccount.Text.Trim().Replace(" ", "");
            if (!IsMatching(accString))
            {
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('你输入的工号和学号格式不正确，请重新输入*^_^*')</script>");
                divAccountMsg.InnerHtml = "你输入的工号和学号格式不正确，请重新输入*^_^*";
                txtAccount.Focus();
                return;
            }
            else
                divAccountMsg.InnerHtml = "";
            int stateid = 0;
            if (UserExits(ref stateid))
            {
                string strMsg = "你输入的工号/学号“ " + accString + " “已注册账户";
                if (stateid == 0)//未激活
                {
                    strMsg += "但被禁用! <br>若有疑问,请联系网站管理员 Email:smartneu@mail.neu.edu.cn;电话: 024-83692103";
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + strMsg + "');</script>");
                  divAccountMsg .InnerHtml = strMsg;
                }
                else//已激活
                {
                    string strUrl_Yes = SPContext.Current.Web.Url + "/_layouts/15/Authenticate.aspx?Source=" + SPContext.Current.Web.Url;
                    string reUrl = "RetrievePwd.aspx?Id="+txtAccount.Text;
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + strMsg + "');</script>");
                    divAccountMsg.InnerHtml = strMsg + "!<br><a href='" + strUrl_Yes + "'>点此直接登录</a>  |  <a href=" + reUrl + ">忘记密码？ 点此找回</a>";
                }
                //用户存在需要加到安全组
                UserAddToGroup();

            }
        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            string currentwebUrl = SPContext.Current.Web.Url;
            Page.Response.Redirect(currentwebUrl);

        }
        
        public static void ShowConfirm(string strMsg, string strUrl_Yes, string strUrl_No)
        {
            System.Web.HttpContext.Current.Response.Write("<Script Language='JavaScript'>if ( window.confirm('" + strMsg + "')) { window.location.href='" + strUrl_Yes +
            "' } else {window.location.href='" + strUrl_No + "' };</script>");
        }
        private bool IsMatching(string accString)
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
        private void UserAddToGroup()
        {
            string account = txtAccount.Text.Trim().Replace(" ", "");
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                string domain = ADHelper.Domain;
                string strConst = HiddenField1.Value;
                if (impersonateValidUser("administrator", domain, strConst.Substring(strConst.IndexOf(" ") + 1)))
                {
                    ADHelper.AddUserToSafeGroup(account, account.Length == 5 ? "SmartNEUTeacher" : "SmartNEUStudent");
                    undoImpersonation();
                }
                else
                {
                    //Your impersonation failed. Therefore, include a fail-safe mechanism here.
                }

            });
        }
        //判断指定的帐户是否存在
        private bool UserExits(ref int stateid)
        {
            int stateint = 0;
            string account = txtAccount.Text.Trim().Replace(" ", "");
            bool retValue = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
               {
                   string domain = ADHelper.Domain;
                   string strConst = HiddenField1.Value;
                   if (impersonateValidUser("administrator", domain, strConst.Substring(strConst.IndexOf(" ") + 1)))
                   {
 
                       retValue = Common.UserExits(account, ref stateint);//stateint                      

                       undoImpersonation();
                   }
                   else
                   {
                       //Your impersonation failed. Therefore, include a fail-safe mechanism here.
                   }

               });
            stateid = stateint;
            return retValue;
        }
        void btnSave_Click(object sender, EventArgs e)
        {
            int stateid = 0;
            try
            {   
                 string account = txtAccount.Text.Trim().Replace(" ", "");
                 if (UserExits(ref stateid))
                 {
                     string strMsg = "你输入的工号/学号“ " + account + " “已经注册";
                     Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + strMsg + "');</script>");
                 }
                 else
                 {
                     int roleID;//1-学生 2-老师
                     if (account.Length == 5)
                         roleID = 2;
                     else
                         roleID = 1;
                     DataSet ds = DAL.User.GetUserByAccount(account);
                     bool succeed = SaveAD(account, txtName.Text.Trim(), txtEmail.Text.Trim(), txtTelephone.Text.Trim(), txtPwd.Text.Trim(), roleID.ToString(), "SmartNEU", true,ddlCity.SelectedItem.Text );
                     if (succeed)
                     {
                         DataRow dr = ds.Tables[0].NewRow();
                         dr["Account"] = account;
                         dr["Name"] = txtName.Text.Trim();
                         //dr["IDCard"] = txtId.Text.Trim();
                         dr["Sex"] = int.Parse(rblSex.SelectedValue);
                         dr["Telephone"] = txtTelephone.Text.Trim();
                         dr["Email"] = txtEmail.Text.Trim();
                         dr["Flag"] = 1;
                         dr["RoleID"] = roleID.ToString();
                         dr["Created"] = DateTime.Now;
                         dr["OrgID"] = int.Parse(ddlCity.SelectedValue);
                         //dr["IP"] = CurrentIP;
                         DAL.User.InsertUser(dr);
                         bool flag = SendEmail();

                         string sexString = "女士";
                         if (int.Parse(rblSex.SelectedValue) == 1)
                         {
                             sexString = "先生";
                         }
                         lbuserName.Text = txtName.Text.Trim() + sexString;
                         lbuseAcc.Text = txtAccount.Text.Trim();
                         lbuserADAcc.Text = "ccc\\" + txtAccount.Text.Trim();
                         lbDateNow.Text = DateTime.Now.ToString("f");
                         divregInfo.Visible = false;
                         divregSuccess.Visible = true;

                     }
                     else
                     {
                         Common.ShowMessage(Page, this.GetType(), "用户注册失败！");
                         divregInfo.Visible = true;
                         divregSuccess.Visible = false;
                     }
                 }
            }
            catch (Exception ex)
            {
                //Common.ShowMessage(Page, this.GetType(), "注册失败！" + ex.ToString());
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('注册失败！" + ex.ToString() + "')</script>");
            }
        }
        #endregion
        //绑定控件
        #region 数据加载
        private void InitControl()
        {
            DataSet dsOrgType = DAL.User.GetOrg(0);
            DataSet dsTmp = dsOrgType.Clone();
            DataRow dr = dsTmp.Tables[0].NewRow();
            dr["OrgID"] = 0;
            dr["OrgName"] = "";
            dsTmp.Tables[0].Rows.Add(dr);
            dsTmp.Merge(dsOrgType);
            //院系分类
            ddlCity.DataTextField = "OrgName";
            ddlCity.DataValueField = "OrgID";
            ddlCity.DataSource = dsTmp;
            ddlCity.DataBind();
            //ddlProvince_SelectedIndexChanged(null, null);

        }
        //院系名称
        void ddlProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DataSet dsCity = DAL.User.GetOrg(int.Parse(ddlProvince.SelectedValue));
            //ddlCity.DataTextField = "OrgName";
            //ddlCity.DataValueField = "OrgID";
            //ddlCity.DataSource = dsCity;
            //ddlCity.DataBind();

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
        private bool SaveAD(string userAccount, string txtName, string txtEmail, string txtTelephone, string txtPwd, string roleName, string schoolName, bool userEnabled,string userDept="")
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
                        string ouName = strConst.Substring(0, strConst.IndexOf(" "));// "iSmart";// System.Configuration.ConfigurationManager.AppSettings["adPath"];
                        ouName = ADHelper.GetDirectoryEntryOfOU("", ouName);
                        retValue = ADHelper.AddUser(userAccount, txtName, txtEmail, txtTelephone, txtPwd, ouName, roleName == "1" ? "SmartNEUStudent" : "SmartNEUTeacher", schoolName, userEnabled,userDept  );
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
        private bool SendEmail()
        {
            bool flag = false;
            string sexString = "女士";
            if (int.Parse(rblSex.SelectedValue) == 1)
            {
                sexString = "先生";
            }
            string dtNow=DateTime.Now.ToString("f");
            string cts = "<div style='font-size:14px;background-color:#F8F8F8;font-family:微软雅黑;width:580px;padding:5px;'><p><b>尊敬的" + txtName.Text.Trim() + sexString + "</b></p><p>您好，欢迎加入智慧东大。</p>";
            cts+= "<p>您的注册信息如下：</p><p align='center'><i>账号："+txtAccount.Text.Trim()  +" 密码："+txtPwd.Text.Trim() +"</i></p>";
            cts+= "<div style='border:#999 solid 1px;'>请注意以下事项：<ul><li>登录时输入的账号格式为：ccc\\" + txtAccount.Text.Trim() + "</li><li>请您一定保管好自己的账号和密码，以防丢失。如需修改密码，请<a href='http://va.neu.edu.cn/SmartNEU/_layouts/15/ChangePassword/ChangePassword.aspx?Source=/SmartNEU/'>点击此处</a></li><li>您在本站发言时请遵守当地法律法规及<a href='http://va.neu.edu.cn/SmartNEU/Pages/Rules.aspx'>本站相关规定</a></li></ul></div><p><p align='right'>智慧东大创意大赛 竞赛委员会</p><p align='right'>" + dtNow + "</p><hr/><p style='font-size:20px;font-weight:600;text-align:center;'><i>SmartNEU: Smart Learning, Smart Work, Smart Life!</i></p></div>";
            string email = WebConfigurationManager.AppSettings["emailFrom"];
            if (email != "")
            {
                string[] mails = Common.getEmailFrom(email);//配置文件信息:邮件地址,邮件密码,邮件smtp服务器地址
                //flag = Common.SendWebMail(mails[0], mails[2], mails[3], txtEmail.Text.Trim(), "智慧东大 - 账户注册成功", content, "1");
                flag = Common.SendMail(mails[0], mails[1], mails[2], new string[] { txtEmail.Text.Trim() }, "智慧东大 - 账户注册成功", cts, mails[3]);
            }
            return flag;
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
    }
}

