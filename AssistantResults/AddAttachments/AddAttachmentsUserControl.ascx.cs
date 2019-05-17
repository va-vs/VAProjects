using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace AssistantResults.AddAttachments
{
    public partial class AddAttachmentsUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AddView();
        }
        public AddAttachments webObj { get; set; }
        private void AddView()
        {
            XsltListViewWebPart xlvWebPart = null;
            //SPSite  spSite =// new SPSite("http://xqx2012/personal/xueqingxia");  
            SPWeb spWeb = SPContext.Current.Web;
            SPList currList = spWeb.Lists.TryGetList(webObj.ListMediaLib);//"文档" );
            //GetList(ref spWeb, ref currList);
            SPView view = currList.Views[0];
            foreach (SPView tmpView in currList.Views)
                if (tmpView.Title == "缩略图")
                {
                    view = tmpView;
                    break;
                }
            xlvWebPart = new XsltListViewWebPart();
            xlvWebPart.ID = "xlvPics";
            xlvWebPart.Toolbar = "None";
            xlvWebPart.WebId =spWeb.ID ;
            xlvWebPart.ListId = currList.ID;
            xlvWebPart.ChromeType = PartChromeType.TitleOnly;
            xlvWebPart.ViewGuid = view.ID.ToString();
            xlvWebPart.ListName = currList.ID.ToString();
            //xlvWebPart.Title = currList.Title;
            xlvWebPart.ClientRender = true;
            xlvWebPart.ListUrl = currList.RootFolder.Url;
            xlvWebPart.CssClass = view.CssStyleSheet;
            xlvWebPart.CssStyleSheet = view.CssStyleSheet;
            xlvWebPart.JSLink = view.JSLink;

            xlvWebPart.AllowClose = false;

            xlvWebPart.AllowConnect = false;

            xlvWebPart.AllowEdit = false;

            xlvWebPart.AllowHide = false;

            xlvWebPart.AllowMinimize = false;

            xlvWebPart.AllowZoneChange = false;

            xlvWebPart.ChromeType = PartChromeType.Default;
            //xlvWebPart.Title = "";
            //xlvWebPart.ViewSelectorFetchAsync = false;
            //xlvWebPart.InplaceSearchEnabled = false;
            //xlvWebPart.ServerRender = false;
            //xlvWebPart.InitialAsyncDataFetch = false;
            //xlvWebPart.PageSize = -1; xlvWebPart.UseSQLDataSourcePaging = true; xlvWebPart.DataSourceID = ""; xlvWebPart.ShowWithSampleData = false; xlvWebPart.AsyncRefresh = false; xlvWebPart.ManualRefresh = false; xlvWebPart.AutoRefresh = false; xlvWebPart.AutoRefreshInterval = 60; xlvWebPart.SuppressWebPartChrome = false; xlvWebPart.ZoneID = "Main"; xlvWebPart.ChromeState = PartChromeState.Normal; xlvWebPart.AllowClose = true; xlvWebPart.AllowZoneChange = true; xlvWebPart.AllowMinimize = true; xlvWebPart.AllowConnect = true; xlvWebPart.AllowEdit = true; xlvWebPart.AllowHide = true; xlvWebPart.Hidden = false;
            xlvWebPart.XmlDefinition = view.GetViewXml();
            xlvWebPart.ParameterBindings = view.ParameterBindings;

            StringBuilder txt = new StringBuilder();
            txt.AppendLine("<div id = \"MSOZoneCell_WebPartWPQ2\" class=\"ms-webpartzone-cell ms-webpart-cell-vertical ms-fullWidth s4-wpcell\" onkeyup=\"WpKeyUp(event)\" onmouseup=\"WpClick(event)\">");
            txt.AppendLine("<div class=\"ms-webpart-chrome ms-webpart-chrome-vertical ms-webpart-chrome-fullWidth \">");
            //txt.AppendLine(xlvWebPart.ToString ());
            //txt.AppendLine("</div></div>");
            try
            {
                this.Controls.Add(new LiteralControl(txt.ToString ()) );
                this.Controls.Add(xlvWebPart );
                this.Controls.Add(new LiteralControl("</div></div>"));
                //divRoll.Controls.Add(xlvWebPart);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.ToString();
            }

        }
        private void GetList(ref SPWeb web,ref SPList list)
        {
            SPWeb _web=null;
            SPList _list=null;
            try
            {
                string personalUrl = "http://localhost/personal/" + Account;//进入个人网站

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite spSite = new SPSite(personalUrl))
                    {
                        using (SPWeb sWeb = spSite.OpenWeb())
                        {
                            sWeb.AllowUnsafeUpdates = true;
                            SPDocumentLibrary library = sWeb.Lists["文档"] as SPDocumentLibrary;
                             _web = sWeb;
                            _list = library;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // "此文档已经存在或者文件类型被限制，请重新选择！";
            }
            web = _web;
            list = _list;
        }
        private string Account
        {
            get
            {
                if (ViewState["account"] == null)
                {
                    SPUser user = SPContext.Current.Web.CurrentUser;
                    string lngAccount = user.LoginName.Substring(user.LoginName.IndexOf("\\") + 1);
                    if (lngAccount == "system") lngAccount = "xueqingxia";
                    ViewState["account"] = lngAccount;
                }
                return ViewState["account"].ToString();
            }
        }
    }
}
