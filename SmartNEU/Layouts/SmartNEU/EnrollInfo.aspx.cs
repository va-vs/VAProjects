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
    public partial class EnrollInfo : LayoutsPageBase
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitControl();
            }
            btnSave.Click += btnSave_Click;
            btnClose.Click += btnClose_Click;
        }


        void btnClose_Click(object sender, EventArgs e)
        {
            string url = SPContext.Current.Web.Url;
            Response.Redirect(url);
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            SaveEnroll();
        }
        #endregion
        #region 方法
        private void SaveEnroll()
        {
            string acc = txtAccount.Text.Trim();
            bool isNum = Common.isNumberic(acc);
            if (isNum && acc.Length >=5 && acc.Length <=8)
            {
                WriteToList("报名", txtCreativeName.Text.Trim(), txtTelephone.Text.Trim(), txtEmail.Text.Trim());
                 SendEmail();
                 string message = "报名信息已经发送到您的邮件，注意查收!";
                 Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('" + message + "');location.href='" + SPContext.Current.Web.Url + "'</script>");
            }
            else
                Common.Alert("您登陆的帐号不是工号/学号，不能报名");
        }
        private void InitControl()
        {
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            if (currentUser == null)
            {
                Common.Alert("请先登录");
                return;
            }

            string account = Common.GetAccount(currentUser.LoginName);
            txtAccount.Text = account;
            DataSet ds = DAL.User.GetUserByAccount(account);
            if (ds.Tables[0].Rows.Count > 0)//数据库中已有该账户信息,则直接从数据库中读取
            {
                DataRow dr = ds.Tables[0].Rows[0];
                txtName.Text = dr["Name"].ToString();
                txtTelephone.Text = dr["Telephone"].ToString();
                txtEmail.Text = dr["Email"].ToString();
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
        private void SendEmail()
        {
            string txtBody = WebConfigurationManager.AppSettings["entrollSendEmailBody"];
            string txtUnit = WebConfigurationManager.AppSettings["entrollSendEmailUnit"];
            txtBody = txtBody.Replace("XXX", txtName.Text);
            string content = txtBody + "<br><br>" + "<span style='float:right'>" + txtUnit + "</span>";
            string email = WebConfigurationManager.AppSettings["emailFrom"];
            if (email != "")
            {
                string[] mails = Common.getEmailFrom(email);
                Common.SendMail(mails[0], mails[1], mails[2], new string[] { txtEmail.Text.Trim() }, "报名成功", content);
            }
        }
        #endregion
        #region 列表方法
        public static void WriteToList(string listName, string creativeName, string tel, string email)
        {
            SPUser lgUser = SPContext.Current.Web.CurrentUser;
            string account = Common.GetAccount(lgUser.LoginName);
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite mySite = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb spWeb = mySite.AllWebs[SPContext.Current.Web.ID])
                    {
                        SPList listNews = spWeb.Lists.TryGetList(listName);
                        if (listNews != null)
                        {
                            SPListItemCollection itemsNews = listNews.Items;

                            SPListItem item = itemsNews.Add();//将新建的博文添加到新闻列表
                            item["创意名称"] = creativeName;
                            item["申报人"] = lgUser.Name;
                            item["工号/学号"] = account;

                            item["联系电话"] = tel;
                            item["电子邮箱"] = email;

                            item.Update();
                        }
                    }
                }
            });
        }
        #endregion
    }
}
