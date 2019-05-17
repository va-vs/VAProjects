using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace FSCWebParts.ListItemNav
{
    public partial class ListItemNavUserControl : UserControl
    {
        private int ItemID
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
                {
                    return int.Parse(Request.QueryString["ID"]);
                }
                else
                {
                    return 0;
                }
            }
        }
        public ListItemNav webpartObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string lstName = webpartObj.ListName;
                if (lstName == "")
                {
                    itemNav.InnerHtml = "未指定列表,无法实现导航";
                    itemNav.Visible = false;
                }
                else
                {
                    GetItemNav(lstName);
                }
            }
        }

        private void GetItemNav(string lstName)
        {
            if (ItemID==0)
            {
                itemNav.InnerHtml = "无法获取当前项目数据";
            }
            else
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()//模拟管理员权限执行，让匿名用户也可以查看此Web部件
                    {
                        string siteUrl = SPContext.Current.Site.Url;
                        using (SPSite mySite = new SPSite(siteUrl))
                        {
                            using (SPWeb myWeb = SPContext.Current.Web)
                            {
                                SPList mylist = myWeb.Lists.TryGetList(lstName);
                                if (mylist != null)
                                {
                                    string queryStr = "";
                                    SPListItem currentItem = mylist.GetItemById(ItemID);
                                    if (currentItem!=null)
                                    {
                                        string flag ="0";
                                        if (currentItem["Flag"]!=null)
	                                    {
                                            flag=currentItem["Flag"].ToString();;
	                                    }
                                        string isOnly = webpartObj.OnlyAudit;//是否只显示待审记录
                                        string showYear = webpartObj.ByYear;
                                        if (string.IsNullOrEmpty(showYear))
                                        {
                                            showYear = DateTime.Now.Year.ToString();
                                        }
                                        if (isOnly == "是")//只显示未审
                                        {
                                            if (flag != "1")//当前未审
                                            {
                                                queryStr += "<Where><And><Eq><FieldRef Name='Year'/><Value Type='Text'>"+ showYear + "</Value></Eq><Eq><FieldRef Name='Flag'/><Value Type='Number'>0</Value></Eq></And></Where>";
                                            }
                                            else//当前已审,不参与导航
                                            {
                                                itemNav.Visible = false;
                                                return;
                                            }
                                            
                                        }
                                        else
                                        {
                                            queryStr += "<Where><Eq><FieldRef Name='Year'/><Value Type='Text'>" + showYear + "</Value></Eq></Where>";
                                        }
                                        
                                        string myurl = mylist.DefaultDisplayFormUrl + "?ID=";
                                        SPQuery qry = new SPQuery();
                                        SPFieldCollection fields = mylist.Fields;
                                        queryStr += GenQuery(fields);
                                        if (queryStr == null)
                                        {
                                            itemNav.InnerHtml = "设置项中的某个字段名设置错误";
                                        }
                                        else
                                        {
                                            qry.Query = queryStr;
                                            SPListItemCollection myItems = mylist.GetItems(qry);//获取sharepoint列表集合
                                            if (myItems.Count > 0)
                                            {
                                                List<SPListItem> listItems = myItems.Cast<SPListItem>().ToList();//将SharePoint列表数据集合转化为普通列表集合
                                                SPListItem myItem = listItems.FirstOrDefault(p => p.ID == ItemID);//查询指定ID的列表项
                                                int index = listItems.IndexOf(myItem);//获取指定列表项的索引
                                                int rCount = myItems.Count;//获取列表的计数
                                                if (rCount >= 2)//多余两条显示导航
                                                {
                                                    if (index == 0)
                                                    {
                                                        //itemindex = myItems[rCount - 1]["ID"].ToString() + ";" + myItems[rCount - 1]["Title"].ToString() + ";"  + myItems[i+1]["ID"].ToString() + ";" + myItems[i+1]["Title"].ToString();
                                                        itemNav.InnerHtml = "<ul><li>当前已是第一条</li><li>&nbsp;&nbsp;共计 <b>" + rCount + "</b> 条</li><li>▽下一条：<a href='" + myurl + myItems[index + 1]["ID"].ToString() + "'>" + GetSubString(myItems[index + 1]["Title"].ToString(), 50) + "</a></li></ul>";
                                                    }
                                                    else if (index == rCount - 1)
                                                    {
                                                        //itemindex = myItems[i - 1]["ID"].ToString() + ";" + myItems[i - 1]["Title"].ToString() + ";" + myItems[0]["ID"].ToString() + ";" + myItems[0]["Title"].ToString();
                                                        itemNav.InnerHtml = "<ul><li>△上一条：<a href='" + myurl + myItems[index - 1]["ID"].ToString() + "'>" + GetSubString(myItems[index - 1]["Title"].ToString(), 50) + "</a></li><li>&nbsp;&nbsp;共计 <b>" + rCount + "</b> 条</li><li>当前已是最后一条</li></ul>";
                                                    }
                                                    else
                                                    {
                                                        //itemindex = myItems[i - 1]["ID"].ToString() + ";" + myItems[i-1]["Title"].ToString()+";"+ myItems[i + 1]["ID"].ToString() + ";" + myItems[i+1]["Title"].ToString();
                                                        itemNav.InnerHtml = "<ul><li>△上一条：<a href='" + myurl + myItems[index - 1]["ID"].ToString() + "'>" + GetSubString(myItems[index - 1]["Title"].ToString(), 50) + "</a></li><li>&nbsp;&nbsp;当前第 <b>" + (index + 1).ToString() + "</b> 条，共计 <b>" + rCount + "</b> 条</li><li>▽下一条：<a href='" + myurl + myItems[index + 1]["ID"].ToString() + "'>" + GetSubString(myItems[index + 1]["Title"].ToString(), 50) + "</a></li></ul>";
                                                    }
                                                }
                                                else
                                                {
                                                    itemNav.Visible = false;//不够两条,无须导航
                                                }
                                            }
                                            else
                                            {
                                                itemNav.Visible = false;//没有数据
                                            }

                                        }
                                    }
                                    
                                    
                                    
                                }
                                else
                                {
                                    itemNav.Visible = false;//列表不存在
                                }
                            }

                        }

                    });
                }
                catch (System.Exception ex)
                {
                    itemNav.InnerHtml = ex.ToString();
                }
            }
        }

        /// <summary>
        /// 根据配置条件生成查询语句
        /// </summary>
        /// <param name="fieldCollection"></param>
        /// <returns></returns>
        private string GenQuery(SPFieldCollection fieldCollection)
        {
            string queryStr = "";
            
            //if (isOnly=="是")
            //{
            //    string myField = webpartObj.MyField;
            //    if (myField=="")//未指定代表用户属性的字段名,缺省为创建者
            //    {
            //        queryStr = "<Where><Eq><FieldRef Name = 'Author'/><Value Type ='Integer'><UserID/></Value ></Eq></Where>";
            //    }
            //    else
            //    {
            //        SPField field = fieldCollection.GetField(myField);
            //        if (field!=null)
            //        {
            //            myField = field.InternalName;
            //            queryStr = "<Where><Eq><FieldRef Name = '" + myField + "'/><Value Type ='Integer'><UserID/></Value></Eq></Where>";
            //        }
            //        else
            //        {
            //            return null;
            //        }   
            //    }                
            //}


            string sortField = webpartObj.SortField;
            string sortDir = webpartObj.SortDirections;//排序方式:0正序,1倒序
            if (sortDir=="0"||sortDir=="")//倒序
            {
                if (sortField == "")//不指定排序字段,缺省以创建时间排序
                {
                    queryStr += "<OrderBy><FieldRef Name='Created' Ascending='FALSE'/></OrderBy>";
                }
                else//指定排序字段sortField
                {
                    SPField field = fieldCollection.GetField(sortField);
                    if (field!=null)
                    {
                        sortField = field.InternalName;
                        queryStr += "<OrderBy><FieldRef Name='" + sortField + "' Ascending='FALSE'/></OrderBy>";
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            else//正序
            {
                if (sortField == "")//不指定排序字段,缺省以创建时间排序
                {
                    queryStr += "<OrderBy><FieldRef Name='Created'/></OrderBy>";
                }
                else//指定排序字段sortField
                {
                    SPField field = fieldCollection.GetField(sortField);
                    if (field != null)
                    {
                        sortField = field.InternalName;
                        queryStr += "<OrderBy><FieldRef Name='" + sortField + "'/></OrderBy>";
                    }
                    else
                    {
                        return null;
                    }
                   
                }
            }
            return queryStr;
        }

        /// <summary> 
        /// 截取文本，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="str">待截取的字符串</param>
        /// <param name="length">需计算长度的字符串</param>
        /// <returns>string</returns>
        public static string GetSubString(string str, int length)
        {
            string temp = str;
            int j = 0;
            int k = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
                {
                    j += 2;
                }
                else
                {
                    j += 1;
                }
                if (j <= length)
                {
                    k += 1;
                }
                if (j > length)
                {
                    return temp.Substring(0, k) + " …";
                }
            }
            return temp;
        }
    }
}
