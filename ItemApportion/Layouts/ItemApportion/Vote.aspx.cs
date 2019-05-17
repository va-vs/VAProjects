using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.Management;

namespace ItemApportion.Layouts.ItemApportion
{
    public partial class Vote : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string itemId = null;
            if (Request.QueryString["ID"]!=null)
            {
                itemId = Request.QueryString["ID"];
            }
            if (itemId!=null)
            {
                GetDataByID(itemId);
            }
            //Cookie["voter"] != null;
            lbIP.Text = GetClientIP();
            IBVote.Click += IBVote_Click;
        }

        private void GetDataByID(string itemId)
        {
            throw new NotImplementedException();
        }

        private void IBVote_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            HttpCookie Cookie = new HttpCookie("voter");
        }


        #region 方法

        /// <summary>
        /// 获取访问者IP地址
        /// </summary>
        /// <returns></returns>
        static public string GetClientIP()
        {
            string userIP = "未获取到用户IP";

            try
            {
                if (HttpContext.Current == null|| HttpContext.Current.Request == null|| HttpContext.Current.Request.ServerVariables == null)
                    return "";

                string CustomerIP = "";

                //CDN加速后取到的IP 
                CustomerIP = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!string.IsNullOrEmpty(CustomerIP))
                    return CustomerIP;

                if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return HttpContext.Current.Request.UserHostAddress;
                return CustomerIP;
            }
            catch { }

            return userIP;
        }

        //public string getMac()
        //{
        //    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");

        //    ManagementObjectCollection moc2 = mc.GetInstances();

        //    foreach (ManagementObject mo in moc2)
        //    {
        //        if ((bool)mo["IPEnabled"] == true)
        //        {
        //            return mo["MacAddress"].ToString();
        //            mo.Dispose();
        //        }
        //    }
        //    return "";
        //}
        #endregion
    }
}
