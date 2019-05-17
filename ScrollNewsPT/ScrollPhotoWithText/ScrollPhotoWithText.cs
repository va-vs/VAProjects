using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ScrollNewsPT.ScrollPhotoWithText
{
    [ToolboxItemAttribute(false)]
    public class ScrollPhotoWithText : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/ScrollNewsPT/ScrollPhotoWithText/ScrollPhotoWithTextUserControl.ascx";

        protected override void CreateChildControls()
        {
            ScrollPhotoWithTextUserControl myuc = Page.LoadControl(_ascxPath) as ScrollPhotoWithTextUserControl;
            if (myuc != null)
                myuc.wpObj = this;
            Controls.Add(myuc);
        }
        #region 属性
        
        /// <summary>
        /// 用户指定新闻列表名称
        /// </summary>
        string _ListName = "外院新闻";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("新闻列表名称")]
        [WebDescription("新闻列表的名称 (例如：新闻)")]
        public string ListName
        {
            get { return _ListName; }
            set { _ListName = value; }
        }

        int _PhotoWidth = 662;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片宽度")]
        [WebDescription("要显示的图片宽度")]
        public int PhotoWidth
        {
            get { return _PhotoWidth; }
            set { _PhotoWidth = value; }
        }
        int _ImagHeight = 340;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片库高度")]
        [WebDescription("要显示的图片高度")]
        public int PhotoHeight
        {
            get { return _ImagHeight; }
            set { _ImagHeight = value; }
        }

        string _SiteUrl = SPContext.Current.Site.Url;//缺省为当前网站的地址
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("网站地址")]
        [WebDescription("新闻列表所在的网站地址，注意：只写到网站的根路径，不需要写到网站首页")]
        public string SiteUrl
        {
            get { return _SiteUrl; }
            set { _SiteUrl = value; }
        }

        uint _NewsNum = 5;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("新闻的个数")]
        [WebDescription("最多在图片滚动中显示的新闻条数，缺省为5个")]
        public uint NewsNum
        {
            get { return _NewsNum; }
            set { _NewsNum = value; }
        }
        #endregion
    }
}
