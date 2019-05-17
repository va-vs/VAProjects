using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace LeaveApproval.Print
{
    public partial class PrintUserControl : UserControl
    {
        public Print WebPartObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            PrintLv();
        }
        #region 事件

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string lvList = WebPartObj.ListName;
            int lvId = int.Parse(Request.QueryString["ID"]);
            string apList = WebPartObj.ResultList;
            ArrayList arrayList = LeaveArray(lvList, lvId, apList);
            if (arrayList.Count==29)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<div style='text-align:center;font-size:16px;font-weight:bold;padding-bottom:10px;'>外国语学院教职工请假审批表</div>");
                sb.Append("<table class='leavetable' width='600' cellspacing='0' cellpadding='0' border='0' align='center'>");
                sb.Append("<tr><td style='width:100px;'>姓名</td><td>" + arrayList[0] + "</td><td>性别</td><td>" + arrayList[1] + "</td><td>部门</td><td>" + arrayList[2] + "</td></tr><tr><td>请假类型</td><td colspan='5'><div style='text-align:left'>" + arrayList[3] + "</div></td></tr>");//姓名、性别、部门、类别共四项，序号：0-3
                sb.Append("<tr><td>请假时间</td><td colspan='5'><div style='text-align:left'>" + arrayList[4] + " 年 " + arrayList[5] + " 月 " + arrayList[6] + " 日 ~ " + arrayList[7] + " 年 " + arrayList[8] + " 月 " + arrayList[9] + " 日</div></td></tr>");//请假时间：开始年月日~结束年月日共六项，序号：4-9
                sb.Append("<tr><td>请假事由</td><td colspan='5'><div style='text-align:left'> " + arrayList[10] + " </div><br/><div style='text-align:right'>申请人签字：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </div><br><div style='text-align:right'> " + arrayList[11] + " 年 " + arrayList[12] + " 月 " + arrayList[13] + " 日</div></td></tr>");//请假事由：10，请假申请年月日：11-13
                sb.Append("<tr><td>所在<br/>系（部、办）<br/>意见</td><td colspan='5'><div style='text-align:left'> " + arrayList[14] + " </div><br/><div style='text-align:right'>负责人签字：" + arrayList[15] + "  </div><br><div style='text-align:right'> " + arrayList[16] + " 年 " + arrayList[17] + " 月 " + arrayList[18] + " 日</div></td></tr>");//负责人签字5项：14-18
                sb.Append("<tr><td>分管<br/>领导<br/>意见</td><td colspan='5'><div style='text-align:left'> " + arrayList[19] + " </div><br/><div style='text-align:right'>领导签字： " + arrayList[20] + " </div><br><div style='text-align:right'> " + arrayList[21] + " 年 " + arrayList[22] + " 月 " + arrayList[23] + " 日</div></td></tr>");//分管领导签字5项：19-23
                sb.Append("<tr><td>人事<br/>领导<br/>意见</td><td colspan='5'><div style='text-align:left'>  " + arrayList[24] + " </div><br/><div style='text-align:right'>领导签字： " + arrayList[25] + " </div><br><div style='text-align:right'> " + arrayList[26] + " 年 " + arrayList[27] + " 月 " + arrayList[28] + " 日</div></td></tr>");//人事领导签字5项：24-28
                sb.Append("</table>");

                printDiv.InnerHtml = sb.ToString();
                printBtn.Visible = true;
                Page.ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), "myscript", "<script>ShowDiv('MyDiv', 'fade');</script>");
            }
            else//未经过审批
            {
                printDiv.InnerHtml = arrayList[0].ToString();
            }
            
            
        }
        #endregion

        #region 方法

        void PrintLv()
        {
            string lvList = WebPartObj.ListName;
            int lvId = int.Parse(Request.QueryString["ID"]);
            string apList = WebPartObj.ResultList;
            ArrayList arrayList = LeaveArray(lvList, lvId, apList);
            if (arrayList.Count == 30)
            {
                viewdiv.Visible = true;
                StringBuilder sb = new StringBuilder();
                
                sb.Append("<div style='text-align:center;font-size:16px;font-weight:bold;padding-bottom:10px;'>外国语学院教职工请假审批表</div>");
                sb.Append("<table class='leavetable' width='600' cellspacing='0' cellpadding='0' border='0' align='center'>");
                int k = 0;
                sb.Append("<tr><td style='width:100px;'>姓名</td><td>" + arrayList[k] + "</td><td>性别</td><td>" + arrayList[k+1] + "</td><td>部门</td><td>" + arrayList[k+2] + "</td></tr><tr><td>请假类型</td><td colspan='5'><div style='text-align:left'>" + arrayList[k+3] + "</div></td></tr>");//姓名、性别、部门、类别共四项，序号：0-3
                k = 4;
                sb.Append("<tr><td>请假时间</td><td colspan='5'><div style='text-align:left'>" + arrayList[k] + " 年 " + arrayList[k+1] + " 月 " + arrayList[k+2] + " 日 ~ " + arrayList[k+3] + " 年 " + arrayList[k+4] + " 月 " + arrayList[k+5] + " 日</div></td></tr>");//请假时间：开始年月日~结束年月日共六项，序号：4-9
                k = 10;
                sb.Append("<tr><td>请假事由</td><td colspan='5'><div style='text-align:left'> " + arrayList[k] + " </div><br/><div style='text-align:right;padding-right:5px;'>申请人签字： " + arrayList[k+1] + " </div><br><div style='text-align:right'> " + arrayList[k+2] + " 年 " + arrayList[k+3] + " 月 " + arrayList[k+4] + " 日</div></td></tr>");//请假事由：10；请假人签字：11；请假申请年月日：12-14；共5项
                k = 15;
                sb.Append("<tr><td>所在<br/>系（部、办）<br/>意见</td><td colspan='5'><div style='text-align:left'> " + arrayList[k] + " </div><br/><div style='padding-right:5px;text-align:right'>负责人签字：" + arrayList[k+1] + "  </div><br><div style='text-align:right'> " + arrayList[k+2] + " 年 " + arrayList[k+3] + " 月 " + arrayList[k+4] + " 日</div></td></tr>");//负责人签字5项：15-19
                k = 20;
                sb.Append("<tr><td>分管<br/>领导<br/>意见</td><td colspan='5'><div style='text-align:left'> " + arrayList[k] + " </div><br/><div style='padding-right:5px;text-align:right'>领导签字： " + arrayList[k+1] + " </div><br><div style='text-align:right'> " + arrayList[k+2] + " 年 " + arrayList[k+3] + " 月 " + arrayList[k+4] + " 日</div></td></tr>");//分管领导签字5项：20-24
                k = 25;
                sb.Append("<tr><td>人事<br/>领导<br/>意见</td><td colspan='5'><div style='text-align:left'>  " + arrayList[k] + " </div><br/><div style='padding-right:5px;text-align:right'>领导签字： " + arrayList[k+1] + " </div><br><div style='text-align:right'> " + arrayList[k+2] + " 年 " + arrayList[k+3] + " 月 " + arrayList[k+4] + " 日</div></td></tr>");//人事领导签字5项：25-29
                sb.Append("</table>");

                printDiv.InnerHtml = sb.ToString();
                printBtn.Visible = true;
                //Page.ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), "myscript", "<script>ShowDiv('MyDiv', 'fade');</script>");
            }
            else//未经过最终审批
            {
                viewdiv.Visible = false;                
            }
        }

        /// <summary>
        /// 根据列表栏的内部名查找显示名
        /// </summary>
        /// <param name="internalName">栏内部名称</param>
        /// <returns></returns>
        public static string getDNameByIName(SPList list, string internalName)
        {
            string dispName = "";
            SPField field = list.Fields.GetFieldByInternalName(internalName);
            dispName = field.Title;
            return dispName;
        }

        /// <summary>
        /// 获取请假申请列表中数据和请假审批列表对应请假记录的审批记录生成请假审批表数据
        /// </summary>
        /// <param name="lvList">请假申请列表名称</param>
        /// <param name="LeaveId">请假记录ID</param>
        /// <param name="apList">请假审批列表名称</param>
        /// <returns></returns>
        protected ArrayList LeaveArray(string lvList, int LeaveId, string apList)
        {
            ArrayList arrayList = new ArrayList();
            string webUrl = WebPartObj.WebUrl != "" ? WebPartObj.WebUrl : "";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.Url)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webUrl))
                    {
                        SPList spList = spWeb.Lists.TryGetList(lvList);
                        if (spList != null)
                        {
                            SPListItem item = spList.GetItemById(LeaveId);//根据请假记录ID查询请假信息

                            string tempstr = item["Flag"] != null ? item["Flag"].ToString() : "0";

                            if (tempstr!="31")
                            {
                                arrayList.Add("该请假尚未经过审批或被拒绝！不能打印审批表！");
                            }
                            else
                            {
                                //请假人 0
                                SPUser user = GetSPUserFromSPListItemByFieldName(item, getDNameByIName(spList, "Author"));
                                arrayList.Add(user.Name);


                                //性别 1
                                tempstr = item[getDNameByIName(spList, "Sex")] == null ? "" : item[getDNameByIName(spList, "Sex")].ToString();
                                arrayList.Add(tempstr);


                                //部门 2
                                tempstr = item[getDNameByIName(spList, "Dept")] == null ? "" : item[getDNameByIName(spList, "Dept")].ToString();
                                arrayList.Add(tempstr);


                                //请假类别 3
                                tempstr = item[getDNameByIName(spList, "Type")] == null ? "" : item[getDNameByIName(spList, "Type")].ToString();
                                arrayList.Add(tempstr);

                                //开始日期：年月日 4-6
                                SplitDatetoArrayList(arrayList, item[getDNameByIName(spList, "StartDate")]);
                                //结束日期：年月日 7-9
                                SplitDatetoArrayList(arrayList, item[getDNameByIName(spList, "EndDate")]);

                                //请假事由 10
                                tempstr = item[getDNameByIName(spList, "Reason")] == null ? "" : item[getDNameByIName(spList, "Reason")].ToString();
                                arrayList.Add(tempstr);

                                //请假人签字 11
                                arrayList.Add(user.Name);

                                //申请日期 12-14
                                SplitDatetoArrayList(arrayList, item[getDNameByIName(spList, "Created")]);

                                //请假审批 15-19，20-24，25-29
                                SPList appList = spWeb.Lists.TryGetList(apList);//请假审批列表
                                if (appList != null)
                                {
                                    for (int i = 1; i <= 3; i++)//Flag=i*10+1
                                    {
                                        string queryStr = @"<Where><And><Eq><FieldRef Name='Flag' /><Value Type='Number'>" + (i * 10 + 1).ToString() + "</Value></Eq><Eq><FieldRef Name='LeaveID' LookupId='True' /><Value Type='Lookup'>" + LeaveId + "</Value></Eq></And></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";
                                        AddApprovData(arrayList, appList, queryStr);
                                    }
                                }
                            }
                        }
                    }
                }
            });
            return arrayList;
        }

        /// <summary>
        /// 将一个日期时间对象拆分成年月日写入ArrayList
        /// </summary>
        /// <param name="arrayList"></param>
        /// <param name="dtObj"></param>
        void SplitDatetoArrayList(ArrayList arrayList, object dtObj)
        {          
            if (dtObj != null)
            {
                DateTime dt = (DateTime)dtObj;
                arrayList.Add(dt.Year);
                arrayList.Add(dt.Month);
                arrayList.Add(dt.Day);
            }
            else
            {
                arrayList.Add("");
                arrayList.Add("");
                arrayList.Add("");
            }
        }

        /// <summary>
        /// 遍历对应级别的审批信息，填入数据集arrayList
        /// </summary>
        /// <param name="arrayList">要填入的数据集ArrayList</param>
        /// <param name="appList">审批列表</param>
        /// <param name="queryStr">查询语句</param>
        protected void AddApprovData(ArrayList arrayList, SPList appList, string queryStr)
        {
            string tempstr = "";
            SPQuery qry = new SPQuery();
            qry.Query = queryStr;//查询请假ID为当前请假申请的ID切对应级别的审批记录
            SPListItemCollection listItems = appList.GetItems(qry);
            if (listItems.Count > 0)//具有待审的请假记录
            {
                SPListItem appitem = listItems[0];
                //审批意见
                tempstr = appitem[getDNameByIName(appList, "Opinion")]== null? " ": appitem[getDNameByIName(appList, "Opinion")].ToString();
                arrayList.Add(tempstr);

                //审批人签字图片地址
                SPUser user= GetSPUserFromSPListItemByFieldName(appitem, getDNameByIName(appList, "Author"));
                string webUrl = WebPartObj.WebUrl != "" ? WebPartObj.WebUrl : "";
                string imgLib= WebPartObj.SignImgLib != "" ? WebPartObj.SignImgLib : "签字图片库";
                tempstr = GetSignImgFromImgLibByTitle(user.LoginName, imgLib, webUrl);                
                arrayList.Add(tempstr);

                //审批日期:年月日
                SplitDatetoArrayList(arrayList, appitem[getDNameByIName(appList, "Created")]);               
            }
            else//没有该级别的审批记录
            {
                arrayList.Add("");
                arrayList.Add("");
                arrayList.Add("");
                arrayList.Add("");
                arrayList.Add("");
            }

        }

        /// <summary>
        /// 查询SharePoint列表项中用户/用户组栏中的用户（单个用户的字段）
        /// </summary>
        /// <param name="spItem">列表项</param>
        /// <param name="fieldName">栏名</param>
        /// <returns></returns>
        SPUser GetSPUserFromSPListItemByFieldName(SPListItem spItem, string fieldName)
        {
            string userName = spItem[fieldName].ToString();
            SPFieldUser _user = (SPFieldUser)spItem.Fields[fieldName];
            SPFieldUserValue userValue = (SPFieldUserValue)_user.GetFieldValue(userName);
            return userValue.User;
        }

        /// <summary>
        /// 根据图片标题从图片库中查找图片
        /// </summary>
        /// <param name="imgTitle">图片标题</param>
        /// <param name="imgLib">图片库名</param>
        /// <param name="webUrl">图片库所在网站</param>
        /// <returns>查找到的图片地址</returns>
        string GetSignImgFromImgLibByTitle(string imgTitle,string imgLib,string webUrl)
        {
            string imgUrl = "";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.Url)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webUrl))
                    {
                        SPList spList = spWeb.Lists.TryGetList(imgLib);
                        if (spList != null)
                        {
                            SPQuery spqry = new SPQuery();
                            spqry.Query = @" <Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + imgTitle + "</Value></Eq></Where>";
                            SPListItemCollection spItems = spList.GetItems(spqry);
                            if (spItems.Count>0)
                            {
                                SPItem item = spItems[0];
                                imgUrl = item["FileRef"] == null ? "" : item["FileRef"].ToString();
                                if (imgUrl!="")
                                {
                                    imgUrl = "<img width='60px' height='40px' src='" + imgUrl+"'/>";
                                }                                
                            }
                        }
                    }
                }
            });
            return imgUrl;
        }


        public void WriteFile()
        {
            //---------------------读html模板页面到stringbuilder对象里---- 
            string[] format = new string[4];//定义和htmlyem标记数目一致的数组 

            StringBuilder htmltext = new StringBuilder();
            try
            {
                using (StreamReader sr = new StreamReader(Server.MapPath(".") + "../../../../_layouts/15/LeaveApproval/template.html")) //模板页路径
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        htmltext.Append(line);
                    }
                    sr.Close();
                }
            }
            catch
            {
                Response.Write("<Script>alert('读取模板文件错误')</Script>");
            }

            //---------------------给标记数组赋值------------ 
            format[0] = "background=\"bg.jpg\"";//背景图片 
            format[1] = "#990099";//字体颜色 
            format[2] = "150px";//字体大小 
            format[3] = "<marquee>生成的模板html页面</marquee>";//文字说明 

            //----------替换htm里的标记为你想加的内容 
            for (int i = 0; i < 4; i++)
            {
                htmltext.Replace("$htmlformat[" + i + "]", format[i]);
            }

            //----------生成htm文件------------------―― 
            try
            {
                using (StreamWriter sw = new StreamWriter(Server.MapPath(".") + "../../../../_layouts/15/LeaveApproval/html/test.html", false, System.Text.Encoding.GetEncoding("GB2312"))) //保存地址
                {
                    sw.WriteLine(htmltext);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
                Response.Write("创建文件失败：禁止写入！");
            }
        }
        #endregion

        protected void btnout_Click(object sender, EventArgs e)
        {
            WriteFile();
        }
    }
}
