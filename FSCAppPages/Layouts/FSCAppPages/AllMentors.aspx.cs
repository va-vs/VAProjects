using System;
using System.Text;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace FSCAppPages.Layouts.FSCAppPages
{
    public partial class AllMentors : UnsecuredLayoutsPageBase
    {
        public static string MentorList = "导师简介";
        public static string MentorRank = "导师排行";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                    FillContents();
            }
            catch (Exception)
            {
                mylist.InnerHtml = "导师信息无法在本站访问!请查证你的访问地址是否正确!";
            }
            
        }
        #region 方法
        private void FillContents()
        {

            StringBuilder txtContent = new StringBuilder();
            string[] cates = new string[] { "英语语言文学", "俄语语言文学", "日语语言文学", "外国语言学及应用语言学", "英语笔译", "英语口译", "日语笔译" };
            SPListItemCollection items;
            string name;
            string zhicheng;
            string majorArea;
            string txt;

            //列表视图显示
            //HtmlGenericControl[] divCate=new HtmlGenericControl[] { div1,div2,div3,div4,div5 ,div6 ,div7 };

            //头像视图显示
            //HtmlGenericControl[] content = new HtmlGenericControl[] { content1, content2, content3, content4, content5, content6, content7};

            mylist.InnerHtml = "";
            for (int i = 0; i < cates.Length; i++)
            {
                txtContent = new StringBuilder();
                string spqueryStr = @" <Where>
                                              <Eq>
                                                 <FieldRef Name='MajorField' />
                                                 <Value Type='LookupField'>" + cates[i] + @"</Value>
                                              </Eq>
                                           </Where>
                                           <OrderBy>
                                              <FieldRef Name='Title' />
                                              <FieldRef Name='MentorName' />
                                           </OrderBy>";
                items = GetItemsBySPQuery(spqueryStr, "导师排行");

                if (items.Count>0)
                {
                    txtContent.AppendLine("<div class='listtitle'>◇ &nbsp;" + cates[i] + "<span>(共&nbsp;<b>" + items.Count + "</b>&nbsp;位)</span></div><div class='pp'>");
                    foreach (SPListItem item in items)
                    {
                        name = item["导师姓名"].ToString();
                        Regex regex = new Regex(";#");//以cjlovefl分割
                        string[] bits = regex.Split(name);
                        SPListItem currentItem=GetItemByID(int.Parse(bits[0]), AllMentors.MentorList);
                        if (currentItem!=null)
                        {
                            if ((bool)currentItem["秦分校"])
                            {
                                name = "(秦分校)";
                            }
                            else
                            {
                                name = "";
                            }
                        }

                        zhicheng = item["导师职称"] == null ? "" : item["导师职称"].ToString();
                        zhicheng = zhicheng.Substring(zhicheng.IndexOf("#")+1);
                        txt = "<a href = 'Mentors.aspx?MentorsID=" + bits[0] + "' alt='' title='" + bits[1] + "&nbsp;·&nbsp;" + zhicheng + "'>" + bits[1]+name + "</a> ";
                        txtContent.AppendLine(txt);
                    }
                    txtContent.AppendLine("</div>");
                    txt = txtContent.ToString();
                    mylist.InnerHtml += txt;
                }
                //else
                //{
                //    txt = "<div class='listtitle'>" + cates[i] + "</div><div class='pp'>无</div>";
                //    mylist.InnerHtml += txt;
                //}


                //头像显示
                //SPItemtoHtmlControl(content[i], items,i+1);
            }

            //新增导师和排行链接
            string newitemurl = HavePermission(MentorList);
            if (newitemurl != "")
            {
                string rankurl = HavePermission(MentorRank);
                addNew.InnerHtml = "<div style='padding:10px;margin-right:10px;font-size:14px;'><a href='" + newitemurl + "' title='添加新的导师'>新增导师</a>&nbsp; | &nbsp;<a href='" + rankurl + "' title='为导师增加排行信息'>新增导师排行</a></div>";
            }
            else
            {
                addNew.Attributes["style"] = "display:none;";
            }


        }



        public SPListItemCollection GetItemsBySPQuery(string spQuery,string listName)
        {
            SPListItemCollection items = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
             {
                 using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                 {
                     using (SPWeb web = spSite.OpenWeb(SPContext.Current.Web.ID))
                     {
                         SPList list = web.Lists.TryGetList(listName);
                         if (list != null)
                         {
                             SPQuery qry = new SPQuery();

                             qry.Query = spQuery;
                             items = list.GetItems(qry);
                         }
                     }
                 }
             });
            return items;
        }

        public SPListItem GetItemByID(int itemId,string listName)
        {
            SPListItem item=null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
             {
                 using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                 {
                     using (SPWeb web = spSite.OpenWeb(SPContext.Current.Web.ID))
                     {
                         SPList list = web.Lists.TryGetList(listName);
                         if (list != null)
                         {
                             try
                             {
                                 item = list.GetItemById(itemId);
                             }
                             catch (Exception)
                             {

                                 item = null;
                             }
                         }
                     }
                 }
             });
            return item;
        }
        public void SPItemtoHtmlControl(HtmlGenericControl htmlcontrol,SPListItemCollection items,int ctrlId)
        {

            string htmlStr = "无";
            if (items.Count>0)
            {
                //htmlStr = "<div id='content" + ctrlId + "'>";
                htmlStr = "<div style='margin-bottom:10px;'>共计<b>&nbsp;"+items.Count+ "&nbsp;</b>位</div>";
                htmlStr += "<ul class='myul'>";
                foreach (SPListItem item in items)
                {
                    string name = item["姓名"].ToString();
                    if ((bool)item["秦分校"])
                        name = name + "(秦分校)";
                    string picurl = item["照片"] == null ? "" : item["照片"].ToString();
                    string[] picurls = GetHtmlImageUrlList(picurl);
                    if (picurls.Length > 0)
                    {
                        picurl = picurls[0];
                    }
                    else
                    {
                        picurl = "images/head.jpg";
                    }

                    string professional = item["职称"] == null ? "" : "<b>职称：</b>" + item["职称"].ToString();
                    string education = item["学历"] == null ? "" : "<b>学历：</b>" + item["学历"].ToString();
                    string area = item["研究方向"] == null ? "" : "<b>研究方向：</b></td><td>" + item["研究方向"].ToString();
                    htmlStr += "<li>";
                    htmlStr += "<table border='0' cellspacing='0' cellpadding='0'>";
                    htmlStr += "<tr>";
                    htmlStr += "<td rowspan='3'><a href ='Mentors.aspx?MentorsID=" + item.ID + "'><img width='140' src='" + picurl + "' border = 'none' alt='' title='" + name + "'></a></td>";
                    htmlStr += "<th cospan='2'><a href='Mentors.aspx?MentorsID=" + item.ID + "'>" + name+"</a></th>";
                    htmlStr += "</tr>";
                    htmlStr += "<tr><td>" + professional + "&nbsp;&nbsp;</td><td>&nbsp;&nbsp;" + education+"</td></tr>";
                    htmlStr += "<tr><td>"+area+"</td></tr>";
                    htmlStr += "</table></li>";
                }
                htmlStr += "</ul>";
                //htmlStr += "</div>";
                htmlcontrol.InnerHtml = htmlStr;

            }
        }

        /// <summary>
        /// 判断当前用户对某一个列表是否有添加项目的权限
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public string HavePermission(string listName)
        {

            string additemUrl="";
            // 得到当前站点
            SPSite currentSite = SPContext.Current.Site;

            //当前页面
            SPWeb currentWeb = currentSite.OpenWeb();

            //当前用户
            SPUser currentUser = currentWeb.CurrentUser;
            if (currentUser != null)
            {
                //判断用户是否在sharepoint组里面
                //提升权限
                SPSecurity.RunWithElevatedPrivileges(
                    delegate ()
                    {
                        //得到后台列表
                        SPList testList = currentWeb.Lists.TryGetList(listName);

                        //判读该用户是否在该列表中有添加权限，（还有其他的几种查看，修改，删除等方法）
                        bool havePermission = testList.DoesUserHavePermissions(currentUser, SPBasePermissions.AddListItems);
                        if (havePermission)
                        {
                            additemUrl = testList.DefaultNewFormUrl + "?Source=" + Request.Url.ToString();
                        }
                    });
            }

            return additemUrl;
        }

        ///<summary>取得HTML中所有图片的 URL。</summary>
        ///<param name="sHtmlText">HTML代码</param>
        ///<returns>图片的URL列表</returns>
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            //定义正则表达式用来匹配 img 标签
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            //搜索匹配的字符串
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];
            //取得匹配项列表
            foreach (Match match in matches)
            {
                sUrlList[i++] = match.Groups["imgUrl"].Value;

            }
            return sUrlList;
        }
        #endregion
        #region 属性
        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true ;
            }
        }
        #endregion
    }
}
