using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

//统计博客的阅读次数
namespace SmartNEUEntrollEvent.StatisticHitCount
{
    [ToolboxItemAttribute(false)]
    public class StatisticHitCount : WebPart
    {
        //http://va.neu.edu.cn/SmartNEU/Lists/Posts/Post.aspx?ID=72
        protected override void CreateChildControls()
        {
            string ID = Page.Request.QueryString["ID"] ;
            string url=Page.Request.Url.OriginalString ;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList list = web.Lists.TryGetList(ListName);
                        if (list == null) return;
                        if (!list.Fields.ContainsField(FieldName))
                            return;
                        SPListItem itme = list.GetItemById(int.Parse(ID));
                        if (itme != null)
                        {
                            if (itme[FieldName] == null)
                                itme[FieldName] = 1;
                            else
                                itme[FieldName] = int.Parse(itme[FieldName].ToString()) + 1;
                            web.AllowUnsafeUpdates = true;

                            itme.SystemUpdate();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                }
        });
            //SPList list=new SPList()
        }
        #region 属性
        /// <summary>
        /// 用户指定新闻列表名称
        /// </summary>
        string _ListName = "创意";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("博客名称")]
        [WebDescription("博客列表的名称 (例如：创意)")]
        public string ListName
        {
            get { return _ListName; }
            set { _ListName = value; }
        }
        string _fieldName = "阅读次数";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("字段名称")]
        [WebDescription("字段名称")]
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }
        #endregion
    }
}
