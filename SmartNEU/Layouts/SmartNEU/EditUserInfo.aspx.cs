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

namespace SmartNEU.Layouts.SmartNEU
{
    public partial class EditUserInfo : LayoutsPageBase
    #region 事件
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                SPUser currentUser = SPContext.Current.Web.CurrentUser;
                if (currentUser == null)
                {
                   lblMsg.Text ="请先登录";
                    return;
                }
                if (ValidRight.UserHaveRight(SPContext.Current.Site.ID,SPContext.Current.Web.Name , currentUser))
                {
                    divAdminuser.Visible = true;
                    btnClose.Visible = true;
                }
                else
                {
                    divAdminuser.Visible = false;
                    btnClose.Visible = false;
                }
                   
                InitControl();
                string account = Common.GetAccount(currentUser.LoginName);
                txtAccount.Text = account;
                FillUserInfo(account);
                
            }
            btnSave.Click += btnSave_Click;
            btnClose.Click += btnClose_Click;
            btnUnSave.Click += BtnUnSave_Click;
            txtAccount.TextChanged += txtAccount_TextChanged;

        }
        
        private void FillUserInfo(string account)
        {
            DataSet ds = DAL.User.GetUserByAccount(account);
            if (ds.Tables[0].Rows.Count > 0)//数据库中已有该账户信息,则直接从数据库中读取
            {
                DataRow dr = ds.Tables[0].Rows[0];
                //string[] drcolStrings = drRow..ToString();
                txtName.Text = dr["Name"].ToString();
                //txtId.Text = dr["IDCard"].ToString();
                rblSex.SelectedValue = dr["Sex"].ToString();
                txtTelephone.Text = dr["Telephone"].ToString();
                txtEmail.Text = dr["Email"].ToString();
                if (dr["OrgID"] != null)
                    ddlCity.SelectedValue = dr["OrgID"].ToString();
            }
            else//该用户信息只在AD中存在,用户信息从AD中读取
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    DirectoryEntry adUser = ADHelper.GetDirectoryEntryByAccount(account);
                    if (adUser != null)
                    {
                        txtName.Text = adUser.Properties["displayName"][0].ToString();
                        if (adUser.Properties.Contains("telephoneNumber"))
                            txtTelephone.Text = adUser.Properties["telephoneNumber"][0].ToString();
                        if (adUser.Properties.Contains("mail"))
                            txtEmail.Text = adUser.Properties["mail"][0].ToString();
                    }
                });
            }
        }
        void txtAccount_TextChanged(object sender, EventArgs e)
        {
            ClearControl();
            string account = txtAccount.Text.Trim().Replace(" ", "");
            if (!Common.IsMatching(account))
            {
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('你输入的工号和学号格式不正确，请重新输入*^_^*');document.forms(0).txtAccount.select();</script>");
                divAccountMsg.InnerHtml = "工号和学号格式不正确，请重新输入*^_^*";
                txtAccount.Focus();
                return;
            }
            if (!UserExits())
            {

                //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('你输入的工号和学号不存在，请重新输入*^_^*');document.forms(0).txtAccount.select();</script>");
                divAccountMsg.InnerHtml = "你输入的工号和学号不存在，请重新输入*^_^*";
                return;
            }
            else
            {
                FillUserInfo(account);
            }
        }
        private bool UserExits()
        {
            string account = txtAccount.Text.Trim().Replace(" ", "");
            bool retValue = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                    DirectoryEntry adUser = ADHelper.GetDirectoryEntryByAccount(account);

                    if (adUser != null)
                    {
                        retValue = true;
                    }
                    else
                        retValue = false;
            });
            return retValue;
        }
        private void BtnUnSave_Click(object sender, EventArgs e)
        {
            string currentwebUrl = SPContext.Current.Web.Url;
            Page.Response.Redirect(currentwebUrl);
        }

        //删除用户信息
        void btnClose_Click(object sender, EventArgs e)
        {
            DeleteAdUser();
        }
        /// <summary>
        /// 提交修改用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string account = txtAccount.Text.Trim().Replace(" ", "");
                DataSet ds = DAL.User.GetUserByAccount(account);
                bool succeed = SaveAd(account, txtName.Text.Trim(), txtEmail.Text.Trim(), txtTelephone.Text.Trim(),ddlCity.SelectedItem.Text );
                if (succeed == true)//AD修改成功,然后对数据库进行操作
                {
                    if (ds.Tables[0].Rows.Count > 0)//在数据库中找到了当前用户的记录,准备更新操作
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        dr["Account"] = account;
                        dr["Name"] = txtName.Text.Trim();
                        //dr["IDCard"] = txtId.Text.Trim();
                        dr["Sex"] = int.Parse(rblSex.SelectedValue);
                        dr["Telephone"] = txtTelephone.Text.Trim();
                        dr["Email"] = txtEmail.Text.Trim();
                        dr["Flag"] = 1;
                        dr["Modified"] = DateTime.Now;
                        dr["OrgID"] = int.Parse(ddlCity.SelectedValue);
                        DAL.User.UpdateUser(dr);//更新当前用户记录
                    }
                    else//该用户不存在数据库中,准备添加当前用户为新用户
                    {
                        DataRow dr = ds.Tables[0].NewRow();
                        dr["Account"] = account;
                        dr["Name"] = txtName.Text.Trim();
                        //dr["IDCard"] = txtId.Text.Trim();
                        dr["Sex"] = int.Parse(rblSex.SelectedValue);
                        dr["Telephone"] = txtTelephone.Text.Trim();
                        dr["Email"] = txtEmail.Text.Trim();
                        dr["Flag"] = 1;
                        dr["Created"] = DateTime.Now;
                        dr["OrgID"] = int.Parse(ddlCity.SelectedValue);

                        DAL.User.InsertUser(dr);//添加新用户纪录
                    }
                    bool isCurrent = IsCurrentUser();
                    if (isCurrent)
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('修改成功');top.location.href='" + SPContext.Current.Web.Url + "'</script>");
                    else
                        lblMsg.Text = "修改成功";
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('修改成功');</script>");
                }
                else
                {
                    //Common.ShowMessage(Page, this.GetType(), "用户信息更新失败！");
                    lblMsg.Text = "用户信息更新失败！";
                }
            }
            catch (Exception ex)
            {
                //Common.ShowMessage(Page, this.GetType(), "用户信息更新失败！" + ex.ToString());
                lblMsg.Text = "用户信息更新失败！" + ex.ToString();
            }
        }
    #endregion
    #region 方法
        private void ClearControl()
        {
            txtName.Text = "";
            txtTelephone.Text = "";
            txtEmail.Text = "";
            ddlCity.SelectedIndex = 0;
            lblMsg.Text = "";
            divAccountMsg.InnerHtml = "";
           
        }
        private bool IsCurrentUser()
        {
            string account = txtAccount.Text.Trim().Replace(" ", "");
            string lgAccount = Common.GetAccount(SPContext.Current.Web.CurrentUser.LoginName);
            if (account == lgAccount)
                return true;
            else
                return false;

        }
        private bool SaveAd(string userAccount, string userName, string userEmail, string userTelephone,string userDept="")
        {
            bool retValue = true;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    retValue = ADHelper.EditUser(userAccount, userName, userEmail, userTelephone,userAccount.Length==5?"SmartNEUTeacher":"SmartNEUStudent",userDept );
                });
                return retValue;
            }
            catch
            {
                return retValue;
            }
        }
        private void DeleteAdUser()
        {
            string account = txtAccount.Text.Trim().Replace(" ", "");
            string lgAccount = Common.GetAccount(SPContext.Current.Web.CurrentUser.LoginName);
            bool retValue=true ;
            SPSecurity.RunWithElevatedPrivileges(delegate()
              {
                  retValue = ADHelper.DeleteAdUser(account);
              });

            if (retValue)//AD修改成功,然后对数据库进行操作
            {
                DataSet ds = DAL.User.GetUserByAccount(account);
                if (ds.Tables[0].Rows.Count > 0)//在数据库中找到了当前用户的记录,准备更新操作
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    dr["Flag"] = 0;
                    DAL.User.UpdateUser(dr);//更新当前用户记录

                }
                SendEmail();
                if (account == lgAccount)//登陆用户和删除用户是同一用户
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('删除用户成功，即将关闭');window.opener=null;window.close();</script>");
                else
                {
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('删除用户成功');</script>");
                    ClearControl();
                    lblMsg.Text = "删除用户成功";
                }
            }
            else
                lblMsg.Text = "删除用户失败！";
                //Common.ShowMessage(Page, this.GetType(), "删除用户失败！");

        }
        private void SendEmail(  )
        {
            SPSite spSite = SPContext.Current.Site;
            SPWeb spWeb = SPContext.Current.Web ;
            string siteName = spWeb.Title .ToString();
            string siteUrl = spWeb.Url.ToString();
            string txtContent = "尊敬的 " + txtAccount.Text.Trim() + " ，您好：<br/><br/>";
            txtContent += "您的帐号在 <b>" + siteName + "</b>于" + string.Format("{0:F}", DateTime.Now) + "被禁用！<br/><br/>如有疑问请联系管理员！";
               string email = WebConfigurationManager.AppSettings["emailFrom"];
               if (email != "")
               {
                   string[] mails = Common.getEmailFrom(email);
                   Common.SendMail(mails[0], mails[1], mails[2], new string[] { txtEmail.Text.Trim() }, "禁用账户", txtContent, mails[3]);
               }
        }
        #endregion 
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
        }
      
        #endregion
        #region 方法
        public const int Logon32LogonInteractive = 2;
        public const int Logon32ProviderDefault = 0;

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
        private bool ImpersonateValidUser(String userName, String domain, String password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(userName, domain, password, Logon32LogonInteractive,
                    Logon32ProviderDefault, ref token) != 0)
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
        private void UndoImpersonation()
        {
            impersonationContext.Undo();
        }
        #endregion

    

    }
    public class ValidRight
    {
        public static bool UserHaveRight(Guid siteID, string webName, SPUser user)
        {
            bool isRight = true;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                {
                    using (SPSite spSite = new SPSite(siteID))
                    {
                        using (SPWeb sWeb = spSite.AllWebs[webName])
                        {
                            isRight = sWeb.DoesUserHavePermissions(user.LoginName, SPBasePermissions.FullMask);
                        }  //return DoesUserHavePermssionsToWeb(ref user, ref sWeb);
                    }
                }
                catch
                {
                    isRight = false;
                }
            });
            return isRight;
        }
    }
}
