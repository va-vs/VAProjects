using Microsoft.SharePoint;
using System;
using System.Data;
using System.Web.UI;

namespace FSCWebParts.Neuec_Index
{
    public partial class Neuec_IndexUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dtList = GetSPListData("语料库简介");
                GetCorpusInfo(dtList);
            }
        }

        private void GetCorpusInfo(DataTable dtList)
        {
            DataRow dr = dtList.Rows[0];
            string desc = DataTableReader.GetString(dr, "Desc");
            string url = DataTableReader.GetString(dr, "ReUrl");
            lbNEUEC.Text = "NEU English Corpus";
            divNEUEC.InnerHtml = desc;

            dr = dtList.Rows[1];
            desc = DataTableReader.GetString(dr, "Desc");
            url = DataTableReader.GetString(dr, "ReUrl");
            lnkNEULC.NavigateUrl = url;
            divNEULC.InnerHtml = desc;

            dr = dtList.Rows[2];
            desc = DataTableReader.GetString(dr, "Desc");
            url = DataTableReader.GetString(dr, "ReUrl");
            lnkNEUAC.NavigateUrl = url;
            divNEUAC.InnerHtml = desc;

        }

        private DataTable GetSPListData(string lstName)
        {
            DataTable dtList = new DataTable();
            SPSecurity.RunWithElevatedPrivileges(delegate ()//模拟管理员权限执行，让匿名用户也可以查看此Web部件
            {
                SPWeb myWeb = SPContext.Current.Web;
                SPList mylist = myWeb.Lists.TryGetList(lstName);
                if (mylist != null)
                {
                    SPQuery qry = new SPQuery()
                    {
                        Query = "<OrderBy><FieldRef Name='ID'/></OrderBy>"
                    };

                    dtList = mylist.GetItems(qry).GetDataTable();
                }
            });
            return dtList;
        }
    }
}
