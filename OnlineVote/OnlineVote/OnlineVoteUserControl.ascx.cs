using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;

namespace OnlineVote.OnlineVote
{
    public partial class OnlineVoteUserControl : UserControl
    {
        public OnlineVote webObj{get;set;} 
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        #region 方法
        public bool AddVoteInfo( string strCreativity)
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.VoteDetail );
                        string ip = getIP();
                        if (spList != null)
                        {
                            spWeb.AllowUnsafeUpdates = true;
                            SPListItem listItem;
                                string ID = Page.Request.QueryString["ID"];
                                listItem = spList.AddItem();
                                listItem["创意名称"] = ID + ";#" + strCreativity;
                                listItem["VoteDate"] = DateTime.Today.ToShortDateString();
                                listItem["IP"] = ip;
                                //listItem["作品状态"] = strAction;
                            listItem.Update();

                            spWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
            return true;
        }
        public int GetVoteInfo(string ip,string strCreativity)
        {
            string siteUrl = SPContext.Current.Site.Url;
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPList spList = spWeb.Lists.TryGetList(webObj.VoteDetail );
                    if (spList != null)
                    {
                        SPQuery qry = new SPQuery();
                        qry.Query = @"<Where><And><And>
                                         <Eq>
                                            <FieldRef Name='Title' />
                                            <Value Type='Text'>"
                            + ip + "</Value></Eq><Eq><FieldRef Name='VoteDate' /><Value Type='DateTime'><Today /></Value></Eq></And><Eq><FieldRef Name='Creativity' /><Value Type='Lookup'>"
                            + strCreativity + "</Value></Eq></And></Where>";

                        SPListItemCollection listItems = spList.GetItems(qry);
                        return listItems.Count;
                    }
                }
            }
            return 0;
        }
        //获取客户端IP地址
        public string getIP()
        {
            string result = String.Empty;
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            if (null == result || result == String.Empty)
            {
                return "0.0.0.0";
            }
            return result;
        }
        /// <summary>
        /// 获取当前要的创意名称
        /// </summary>
        /// <returns></returns>
        private string GetCreativityName()
        {
            string ID = Page.Request.QueryString["ID"];
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(webObj.ListName);
            if (list != null)
            {
                SPListItem itme = list.GetItemById(int.Parse(ID));
                if (itme != null)
                    return itme["创意名称"].ToString();
            }
            return "";
        }
        string _creativty;
        public string creatity
        {
            get
            {
                if (_creativty == null)
                    _creativty = GetCreativityName();
                return _creativty;
            }
            set { _creativty = value; }
        }
        #endregion
    }
}
