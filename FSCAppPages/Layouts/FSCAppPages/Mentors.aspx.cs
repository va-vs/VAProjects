using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text.RegularExpressions;
using System.Text;

namespace FSCAppPages.Layouts.FSCAppPages
{
    public partial class Mentors :UnsecuredLayoutsPageBase
    {
        public static string MentorList = "导师简介";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                FillContent();

        }
        #region 方法
        private void FillContent()
        {
            int id = MentorsID;
            if (id == 0) return;

            SPListItem item=null ;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
           {
               using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                {
                   using (SPWeb web = spSite.OpenWeb(SPContext.Current.Web.ID ))
                   {
                       SPList list = web.Lists.TryGetList(Mentors.MentorList);
                       if (list != null)
                       {
                           item = list.Items.GetItemById(id);
                           if (HavePermission(Mentors.MentorList))
                           {
                               string editurl = list.DefaultEditFormUrl;
                               editlink.InnerHtml = "<a href='"+ editurl + "?ID=" + id + "&Source=" + Request.Url.ToString() + "'>修改当前导师信息</a>";
                           }
                       }

                   }
               }
           });
            if (item!=null)
            {
                mtName.InnerText = item["姓名"] == null ? "" : item["姓名"].ToString();


                string name= item["姓名"] == null ? "" : item["姓名"].ToString();
                if ((bool)item["秦分校"])
                {
                    name = "<span title=' 秦皇岛分校导师 '>"+name+" &nbsp ✲</span>";
                    msgdiv.Visible = true;
                }

                divName.InnerHtml = name;
                lblSex.InnerText = item["性别"] == null ? "" : item["性别"].ToString();
                lblBirthDate.InnerText = item["出生年月"] == null ? "" : item["出生年月"].ToString();
                lblEducation.InnerText = item["学历"] == null ? "" : item["学历"].ToString();
                lblProfessional.InnerText = item["职称"] == null ? "" : item["职称"].ToString();
                lblDuties.InnerText = item["职务"] == null ? "" : item["职务"].ToString();
                divPhoto.InnerHtml = item["照片"] == null ? "" : item["照片"].ToString();
                divResearch.InnerHtml = item["研究方向"] == null ? "" : item["研究方向"].ToString();
                divCourse.InnerHtml = item["开设主要课程"] == null ? "" : item["开设主要课程"].ToString();
                divResults.InnerHtml = item["代表性研究成果"] == null ? "" : item["代表性研究成果"].ToString();

                string[] areas = GetMajorAreaByName(item["姓名"].ToString());
                string areastr = "";
                for (int i = 0; i < areas.Length; i++)
                {
                    areastr += areas[i]+"、";
                }
                areastr=areastr.TrimEnd('、');
                GetSameCate(areas, item["姓名"] == null ? "" : item["姓名"].ToString());
                divMajorArea.InnerText = areastr;
            }
        }


        public string[] GetMajorAreaByName(string name)
        {
            string[] majorarea=null;
            string queryStr = @"<Where>
                                  <Eq>
                                     <FieldRef Name='MentorName' />
                                     <Value Type='Lookup'>" + name + @"</Value>
                                  </Eq>
                               </Where>";
            SPListItemCollection items = GetItemsBySPQuery(queryStr, "导师排行");
            if (items != null)
            {

                majorarea = new string[items.Count];
                int i = 0;
                string area;
                foreach (SPListItem item in items)
                {
                    area=item["专业方向"] == null ? "" : item["专业方向"].ToString();
                    area = area.Substring(area.IndexOf('#')+1);
                    majorarea[i] += area;
                    i++;
                }
            }
            return majorarea;
        }

        public string SPItemToString(object fieldcontent)
        {
            string itemstr = "";
            if (fieldcontent != null)
            {
                itemstr = fieldcontent.ToString();
            }
            return itemstr;
        }

        public void GetSameCate(string[] areas,string currentName)
        {
            if (areas.Length>0)
            {
                StringBuilder txtContent = new StringBuilder();
                string name;
                string txt;
                int allCount = 0;
                for (int i = 0; i < areas.Length; i++)
                {
                    string spqueryStr = @"<Where>
                                          <And>
                                             <Eq>
                                                <FieldRef Name='MajorField' />
                                                <Value Type='Lookup'>"+ areas[i] + @"</Value>
                                             </Eq>
                                             <Neq>
                                                <FieldRef Name='MentorName' />
                                                <Value Type='Lookup'>" + currentName + @"</Value>
                                             </Neq>
                                          </And>
                                       </Where>
                                       <OrderBy>
                                          <FieldRef Name='Title' />
                                          <FieldRef Name='MentorName' />
                                       </OrderBy>";
                    SPListItemCollection items = GetItemsBySPQuery(spqueryStr, "导师排行");
                    if (items.Count>0)
                    {
                        allCount += items.Count;
                        txtContent.AppendLine("<div class='listtitle'>◇ &nbsp;" + areas[i] + "</div>");
                        txtContent.AppendLine("<div class='pp'><ul>");
                        Regex regex = new Regex(";#");//以;#分割
                        foreach (SPListItem item in items)
                        {
                            name = item["导师姓名"].ToString();

                            string[] mentor = regex.Split(name);
                            SPListItem currentItem = GetItemByID(int.Parse(mentor[0]),Mentors.MentorList);
                            if (currentItem != null)
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
                            txt = "<li><a href = 'Mentors.aspx?MentorsID=" + mentor[0] + "' alt='' title='点击查看详情'>" + mentor[1] + name + "</a></li>";
                            txtContent.AppendLine(txt);

                        }
                        txtContent.AppendLine("</ul></div>");
                    }
                }

                rsdirect.InnerHtml = txtContent.ToString();
                if (allCount<=0)
                {
                    samelist.Visible = false;
                }
            }

        }


        public SPListItemCollection GetItemsBySPQuery(string spQuery, string listName)
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

        public SPListItem GetItemByID(int itemId, string listName)
        {
            SPListItem item = null;
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


        /// <summary>
        /// 获取当前导师每个专业方向下的同方向的其他导师（不包含本人）
        /// </summary>
        /// <param name="researchCate"></param>
        /// <returns></returns>
        private SPListItemCollection GetMentorsByCate(string researchCate)
        {

            SPListItemCollection items = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
         {
             using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
             {
                 using (SPWeb web = spSite.OpenWeb(SPContext.Current.Web.ID))
                 {
                     SPList list = web.Lists.TryGetList(Mentors.MentorList);
                     if (list != null)
                     {
                         int id = MentorsID;

                         SPQuery qry = new SPQuery();
                         qry.ViewFields = @"<FieldRef Name='Title' /><FieldRef Name = 'ID' /><FieldRef Name = 'BranchCampus' /> ";

                         if (id == 0)

                         {
                             qry.Query = @"<Where>
                                          <Eq>
                                             <FieldRef Name='ResearchDirectionCate' />
                                             <Value Type='MultiChoice'>" + researchCate + @"</Value>
                                          </Eq>
                                       </Where>";
                         }
                         else
                         {
                             qry.Query = @"<Where>
                                              <And>
                                                 <Neq>
                                                    <FieldRef Name='ID' />
                                                    <Value Type='Counter'>" + id + @"</Value>
                                                 </Neq>
                                                 <Eq>
                                                    <FieldRef Name='ResearchDirectionCate'/>
                                                    <Value Type='MultiChoice'>" + researchCate + @"</Value>
                                                 </Eq>
                                              </And>
                                           </Where>
                                           <OrderBy>
                                              <FieldRef Name='Rates' Ascending='FALSE' />
                                              <FieldRef Name='Rates' Ascending='FALSE' />
                                           </OrderBy>";
                         }
                         items = list.GetItems(qry);

                     }
                 }
             }
         });
            return items;
        }

        /// <summary>
        /// 判断当前用户对某一个列表是否有修改项目的权限
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public bool HavePermission(string listName)
        {
            bool havePermission = false;

            // 得到当前站点
            SPSite currentSite = SPContext.Current.Site;

            //当前页面
            SPWeb currentWeb = currentSite.OpenWeb();

            //当前用户
            SPUser currentUser = currentWeb.CurrentUser;
            if (currentUser!=null)
            {
                //判断用户是否在sharepoint组里面
                //提升权限
                SPSecurity.RunWithElevatedPrivileges(
                    delegate ()
                    {
                    //得到后台列表
                    SPList testList = currentWeb.Lists.TryGetList(listName);

                        //判读该用户是否在该列表中有添加权限，（还有其他的几种查看，修改，删除等方法）
                        havePermission = testList.DoesUserHavePermissions(currentUser, SPBasePermissions.EditListItems);
                    });
            }

            return havePermission;
        }

        /// <summary>
        /// 替换字符串起始位置(开头)中指定的字符串
        /// </summary>
        /// <param name="s">源串</param>
        /// <param name="searchStr">查找的串</param>
        /// <param name="replaceStr">替换的目标串</param>
        /// <returns></returns>
        public static string TrimStarString(string s, string searchStr, string replaceStr)
        {
            var result = s;
            try
            {
                if (string.IsNullOrEmpty(result))
                {
                    return result;
                }
                if (s.Length < searchStr.Length)
                {
                    return result;
                }
                if (s.IndexOf(searchStr, 0, searchStr.Length, StringComparison.Ordinal) > -1)
                {
                    result = s.Substring(searchStr.Length);
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
        /// <summary>
        /// 替换字符串末尾位置中指定的字符串
        /// </summary>
        /// <param name="s">源串</param>
        /// <param name="searchStr">查找的串</param>
        /// <param name="replaceStr">替换的目标串</param>
        public static string TrimEndString(string s, string searchStr, string replaceStr)
        {
            var result = s;
            try
            {
                if (string.IsNullOrEmpty(result))
                {
                    return result;
                }
                if (s.Length < searchStr.Length)
                {
                    return result;
                }
                if (s.IndexOf(searchStr, s.Length - searchStr.Length, searchStr.Length, StringComparison.Ordinal) > -1)
                {
                    result = s.Substring(0, s.Length - searchStr.Length);
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }


        #endregion
        #region 属性
        public int MentorsID
        {
            get
            {
                int mentorID = 0;
                if (Request.QueryString["MentorsID"] != null)
                {
                    mentorID = int.Parse(Request.QueryString["MentorsID"]);
                }
                return mentorID;
            }
        }


        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true;
            }
        }
        #endregion
    }
}
