using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;

namespace FSCAppPages.Layouts.FSCAppPages.Corpus
{
    public partial class neuac : LayoutsPageBase
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.AutoGenerateColumns = true;
            btnTest.Click += BtnTest_Click;
            //if (!Page.IsPostBack)
            //    test();   
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            test();
        }
        #endregion
        #region 方法
        private void test()
        {
            string txt = FSCDLL.DAL.Corpus.GetCorporaByID(1).Tables[0].Rows[0]["OriginalText"].ToString ()  ;  
            DataTable  dt= Common.GetClusterFromCorpus(txt,3);
            GridView1.DataSource = dt.DefaultView ;
            GridView1.DataBind();
        }
        #endregion
    }
}
