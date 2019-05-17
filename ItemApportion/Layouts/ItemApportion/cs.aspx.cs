using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace ItemApportion.Layouts.ItemApportion
{
    public partial class cs : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            gvCs.DataSource = getDataBySPList("创意");
            gvCs.DataBind();
        }

        /// <summary>
        /// 读取创意列表生成创意DataTable数据源
        /// </summary>
        /// <param name="lstName"></param>
        /// <returns></returns>
        private DataTable getDataBySPList(string lstName)
        {
            DataTable dt = new DataTable("创意表");
            using (SPSite site = SPContext.Current.Site)
            {
                using(SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPListCollection lstColl = web.Lists;
                    SPList lst = lstColl.TryGetList(lstName);
                    if (lst!=null)
                    {
                        SPQuery qry = new SPQuery();
                        qry.Query =@"<Where><Eq><FieldRef Name='Flag0' /><Value Type='Number'>1</Value></Eq></Where>";
                        SPListItemCollection lstItems = lst.GetItems(qry);
                        if (lst.ItemCount>0)
                        {
                            dt = lstItems.GetDataTable();
                        }
                        else
                        {
                            dt = null;
                        }
                    }
                    
                }
            }           
            return dt;
        }

        private void GetAData(string lstName)
        {
            DataTable dt = getDataBySPList(lstName);
        }

        private void gvData()
        {

        }

        //private string GetIEMI()
        //{
        //    string iemi = "";
        //    String cmd = Application.StartupPath + "\\adb\\adb.exe";

        //    Process p = new Process();
        //    ///////////////////////////方法1
        //    p.StartInfo = new System.Diagnostics.ProcessStartInfo();
        //    p.StartInfo.FileName = cmd;//设定程序名  
        //    p.StartInfo.Arguments = " shell getprop ro.product.model";
        //    p.StartInfo.UseShellExecute = false; //关闭shell的使用  
        //    p.StartInfo.RedirectStandardInput = true; //重定向标准输入  
        //    p.StartInfo.RedirectStandardOutput = true; //重定向标准输出  
        //    p.StartInfo.RedirectStandardError = true; //重定向错误输出  
        //    p.StartInfo.CreateNoWindow = true;//设置不显示窗口  
        //    p.Start();
        //    label2.Text = p.StandardOutput.ReadToEnd();
        //    p.Close();
        //    ///////////////////////////方法2
        //    p.StartInfo = new System.Diagnostics.ProcessStartInfo();
        //    p.StartInfo.FileName = cmd;//设定程序名  
        //    p.StartInfo.Arguments = " shell dumpsys iphonesubinfo";
        //    p.StartInfo.UseShellExecute = false; //关闭shell的使用  
        //    p.StartInfo.RedirectStandardInput = true; //重定向标准输入  
        //    p.StartInfo.RedirectStandardOutput = true; //重定向标准输出  
        //    p.StartInfo.RedirectStandardError = true; //重定向错误输出  
        //    p.StartInfo.CreateNoWindow = true;//设置不显示窗口  
        //    p.Start();
        //    preimei = p.StandardOutput.ReadToEnd();
        //    string[] sArray = preimei.Split(new char[1] { '=' });
        //    imei = sArray[2].Trim();
        //    p.Close();
        //    return iemi;
        //}
       
    }
}
