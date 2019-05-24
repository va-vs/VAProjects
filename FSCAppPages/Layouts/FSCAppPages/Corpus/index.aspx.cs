using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{
    public partial class index : UnsecuredLayoutsPageBase
    {
        //语料库
        protected void Page_Load(object sender, EventArgs e)
        {
            int userId = FSCDLL.Common.Users.UserID;
            if (userId == 0)//未登录
            {
                divIndex.Visible = false;
                lbErr.Text = "本系统不支持匿名使用，请先登录！";
            }
            else
            {
                divIndex.Visible = true;
                lbErr.Text = "";
            }
        }
    }
}
