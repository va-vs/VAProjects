using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace LeaveApproval.PrintTable
{
    public partial class PrintTableUserControl : UserControl
    {
        public PrintTable WebPartObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl myCss = new HtmlGenericControl();
            myCss.TagName = "link";
            myCss.Attributes.Add("type", "text/css");
            myCss.Attributes.Add("rel", "stylesheet");
            myCss.Attributes.Add("href", ResolveUrl(Page.ResolveClientUrl("../../../../_layouts/15/LeaveApproval/css/LeaveListCSS.css")));
            this.Page.Header.Controls.AddAt(0, myCss);

        }
        #region 请假表
        protected ArrayList LeaveArray(string lvList, int LeaveId, string apList)
        {
            ArrayList arrayList = new ArrayList();
           
            string webUrl = WebPartObj.SiteUrl != "" ? WebPartObj.SiteUrl : "";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.Url)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webUrl))
                    {
                        SPList spList = spWeb.Lists.TryGetList(lvList);
                        if (spList != null)
                        {
                            SPListItem item = spList.GetItemById(LeaveId);//查询当前请假信息
                            string tempstr = "";
                            tempstr = item[getDNameByIName(spList, "Flag")] == null ? "" : item[getDNameByIName(spList, "Flag")].ToString();
                            if (tempstr!="31")
                            {
                                arrayList.Add("未经审批的请假，不可打印！");
                            }
                            else
                            {
                                //DataTable leaveData = createdTable();
                                //创建一行
                                //DataRow row = leaveData.NewRow();

                                //请假人 0
                                //tempstr = item[getDNameByIName(spList, "Author")] == null ? "" : item[getDNameByIName(spList, "Author")].ToString();
                                SPUser user = GetSPUserFromSPListItemByFieldName(item, getDNameByIName(spList, "Author"));
                                tempstr = user.Name;
                                arrayList.Add(tempstr);
                                //row[0] = tempstr;

                                //性别 1
                                tempstr = item[getDNameByIName(spList, "Sex")] == null ? "" : item[getDNameByIName(spList, "Sex")].ToString();
                                arrayList.Add(tempstr);
                                //row[1] = tempstr;

                                //部门 2
                                tempstr = item[getDNameByIName(spList, "Dept")] == null ? "" : item[getDNameByIName(spList, "Dept")].ToString();
                                arrayList.Add(tempstr);
                                //row[2] = tempstr;

                                //请假类别 3
                                tempstr = item[getDNameByIName(spList, "Type")] == null ? "" : item[getDNameByIName(spList, "Type")].ToString();
                                arrayList.Add(tempstr);
                                //row[3] = tempstr;

                                //请假时间
                                //A开始时间：年月日 4 5 6
                                SplitDateObject(item[getDNameByIName(spList, "StartDate")], arrayList);

                                //B结束时间：年月日 7 8 9
                                SplitDateObject(item[getDNameByIName(spList, "EndDate")], arrayList);

                                //请假事由 10
                                tempstr = item[getDNameByIName(spList, "Reason")] == null ? "" : item[getDNameByIName(spList, "Reason")].ToString();
                                arrayList.Add(tempstr);
                                //row[4] = tempstr;

                                //申请时间：年月日 11 12 13                            
                                SplitDateObject(item[getDNameByIName(spList, "Created")], arrayList);

                                //审批意见、签字、签字时间（年月日）14-18；19-23；24-28
                                SPList appList = spWeb.Lists.TryGetList(apList);//请假审批列表
                                if (appList != null)
                                {
                                    for (int i = 1; i <= 3; i++)//Flag=i*10+1
                                    {
                                        string queryStr = @"<Where><And><Eq><FieldRef Name='Flag' /><Value Type='Number'>" + (i * 10 + 1).ToString() + "</Value></Eq><Eq><FieldRef Name='LeaveID' LookupId='True' /><Value Type='Lookup'>" + LeaveId + "</Value></Eq></And></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";
                                        AddListData(arrayList, appList, queryStr);
                                    }
                                }
                                //将此行添加到table中
                                //leaveData.Rows.Add(row);
                            }
                        }
                    }
                }
            });
            return arrayList;
        }

        /// <summary>
        /// 获取SharePoint列表项中的用户/用户组栏中用户名
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

        string GetUserSignImageByUser(SPUser user)
        {
            string imgUrl = "http://sp01/SignImg/";
            imgUrl=WebPartObj.ImgUrl==""?imgUrl: WebPartObj.ImgUrl;
            string userName = user.LoginName;
            string userImage = "<img src='"+ imgUrl+ userName + ".jpg' height='40' width='60'/>";
            return userImage;
        }

        protected void SplitDateObject(object dtobj,ArrayList arrayList)
        {
            DateTime dt0 = DateTime.Now.AddDays(1);
            DateTime dt = dtobj == null ? dt0 : (DateTime)dtobj;
            if (dt == dt0)
            {
                arrayList.Add("");
                arrayList.Add("");
                arrayList.Add("");
            }
            else
            {
                string tempstr = "";
                //日期-年
                tempstr = dt.Year.ToString();
                arrayList.Add(tempstr);

                //日期-月
                tempstr = dt.Month.ToString();
                arrayList.Add(tempstr);

                //日期-日
                tempstr = dt.Day.ToString();
                arrayList.Add(tempstr);
            }
        }

        protected void AddListData(ArrayList arrayList, SPList appList, string queryStr)
        {
            string tempstr = "";
            SPQuery qry = new SPQuery();
            qry.Query = queryStr;//查询请假ID为当前请假申请的ID切对应级别的审批记录
            SPListItemCollection listItems = appList.GetItems(qry);
            if (listItems.Count > 0)//具有待审的请假记录
            {
                SPListItem appitem = listItems[0];
                //审批意见
                tempstr =  appitem[getDNameByIName(appList, "Opinion")] == null ? "" : appitem[getDNameByIName(appList, "Opinion")].ToString();
                arrayList.Add(tempstr);

                //审批人
                SPUser user =GetSPUserFromSPListItemByFieldName(appitem, getDNameByIName(appList, "Author"));
                tempstr = GetUserSignImageByUser(user);
                arrayList.Add(tempstr);

                //审批日期
                object dtobj = appitem[getDNameByIName(appList, "Created")];
                SplitDateObject(dtobj,arrayList);
                  
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

        protected DataTable createdTable()
        {
            string[] fields = { "Name", "Sex", "Dept", "Type", "Year1", "Month1", "Day1", "Year2", "Month2", "Day2", "Reason", "LeaveYear", "LeaveMonth", "LeaveDay", "Opinion1", "Sign1", "SignYear1", "SignMonth1", "SignDay1", "Opinion2", "Sign2", "SignYear2", "SignMonth2", "SignDay2", "Opinion3", "Sign3", "SignYear3", "SignMonth3", "SignDay3" };//29项
            DataTable tblleaveData = new DataTable("leaveData");
            for (int i = 0; i < fields.Length; i++)
            {
                tblleaveData.Columns.Add(fields[i], Type.GetType("System.String"));
            }
            return tblleaveData;
        }

        /// <summary>
        /// 根据列表栏的内部名查找显示名
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns></returns>
        public static string getDNameByIName(SPList list, string internalName)
        {
            string dispName = "";
            SPField field = list.Fields.GetFieldByInternalName(internalName);
            dispName = field.Title;
            return dispName;
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string lvList = WebPartObj.ListName;
            int lvId = int.Parse(Request.QueryString["ID"]);
            string apList = WebPartObj.ResultList;
            ArrayList arrayList = LeaveArray(lvList, lvId, apList);
            if (arrayList.Count==29)
            {            
                StringBuilder sb = new StringBuilder();

                sb.Append("<h2 style='text-align:center'>外国语学院教职工请假审批表</h2>");
                sb.Append("<table class='leavetable' width='600' cellspacing='0' cellpadding='0' border='0' align='center'>");
                sb.Append("<tr><td style='width:100px;'>姓名</td><td>" + arrayList[0] + "</td><td>性别</td><td>" + arrayList[1] + "</td><td>部门</td><td>" + arrayList[2] + "</td></tr><tr><td>请假类型</td><td colspan='5'><div style='text-align:left'>" + arrayList[3] + "</div></td></tr>");//姓名、性别、部门、类别共四项，序号：0-3
                sb.Append("<tr><td>请假时间</td><td colspan='5'><div style='text-align:left'>" + arrayList[4] + " 年 " + arrayList[5] + " 月 " + arrayList[6] + " 日 ~ " + arrayList[7] + " 年 " + arrayList[8] + " 月 " + arrayList[9] + " 日</div></td></tr>");//请假时间：开始年月日~结束年月日共六项，序号：4-9
                sb.Append("<tr><td>请假事由</td><td colspan='5'><div style='text-align:left'> " + arrayList[10] + " </div><br/><div style='text-align:right'>申请人签字：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </div><br><div style='text-align:right'> " + arrayList[11] + " 年 " + arrayList[12] + " 月 " + arrayList[13] + " 日</div></td></tr>");//请假事由：10，请假年月日：11-13
                sb.Append("<tr><td>所在<br/>系（部、办）<br/>意见</td><td colspan='5'><div style='text-align:left'> " + arrayList[14] + " </div><br/><div style='text-align:right'>负责人签字：" + arrayList[15] + "  </div><br><div style='text-align:right'> " + arrayList[16] + " 年 " + arrayList[17] + " 月 " + arrayList[18] + " 日</div></td></tr>");//负责人签字5项：14-18
                sb.Append("<tr><td>分管<br/>领导<br/>意见</td><td colspan='5'><div style='text-align:left'> " + arrayList[19] + " </div><br/><div style='text-align:right'>领导签字： " + arrayList[20] + " </div><br><div style='text-align:right'> " + arrayList[21] + " 年 " + arrayList[22] + " 月 " + arrayList[23] + " 日</div></td></tr>");//分管领导签字5项：19-23
                sb.Append("<tr><td>人事<br/>领导<br/>意见</td><td colspan='5'><div style='text-align:left'>  " + arrayList[24] + " </div><br/><div style='text-align:right'>领导签字： " + arrayList[25] + " </div><br><div style='text-align:right'> " + arrayList[26] + " 年 " + arrayList[27] + " 月 " + arrayList[28] + " 日</div></td></tr>");//人事领导签字5项：24-28
                sb.Append("</table>");
                printDiv.InnerHtml = sb.ToString();
                printBtn.Visible = true;
            }
           
        }
        #endregion

    }
}
