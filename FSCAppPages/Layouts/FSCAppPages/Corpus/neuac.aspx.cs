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
            string txt = FSCDLL.DAL.Corpus.GetCorporaByID(1163).Tables[0].Rows[0]["OriginalText"].ToString();
            //            //@"Here are my reasons.
            //First,cellphone will on our study."; 
            try
            {
                for (int i = 0; i < 200; i++)
                    txt = txt+ txt;

                lbErr.Text = "total counts:"+txt.Length ;
            }
            catch
            {
                lbErr.Text ="error total:"+ txt.Length.ToString();
            }
            DateTime dt1 = DateTime.Now;
            DataTable dt =GetClusterFromCorpus(txt, 2);
            lbErr.Text = lbErr.Text + "共用时间为秒" + DateTime.Now.Subtract(dt1).TotalSeconds.ToString(); 
            ////DataRow[] drs=dt.Select ("");
            ////drs.CopyToDataTable();
            ////dt.AsEnumerable().Take(10).CopyToDataTable<DataRow>();
            //GridView1.DataSource = dt.DefaultView;
            //GridView1.DataBind();
        }
        private   DataTable GetClusterFromCorpus(string corpusContext, int ClusterLength)
        {
            DataTable dt = new DataTable();
            List<string> words =Common. ParseWords(corpusContext);
            dt.Columns.Add("Cluster", typeof(string));
            dt.Columns.Add("Count", typeof(int));
            DataRow dr;
            DataRow[] drs;
            int i = 0;
            string cluster;
            while (i <= words.Count - ClusterLength)
            {
                cluster =  words[i] ;
                drs = dt.Select("Cluster='" + cluster + "'");
                if (drs.Length > 0)
                {
                    dr = drs[0];
                    dr["Count"] = int.Parse(dr["Count"].ToString()) + 1;
                }
                else
                {
                    try
                    {
                        dr = dt.Rows.Add();
                        dr["Cluster"] = cluster;
                        dr["Count"] = 1;
                    }
                    catch (Exception ex)
                    {
                        lbErr.Text = ex.ToString() + " " + dt.Rows.Count.ToString();
                    }
                }
                i = i + 1;
            }
            return dt;

        }
        #endregion
    }
}
