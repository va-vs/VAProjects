using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace FSCWebParts.PerformanceSummary
{
    public partial class PerformanceSummaryUserControl : UserControl
    {
        public PerformanceSummary webObj;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GetCollateDates();
                if (!IsPostBack)
                {
                    BindYears();
                    SPUser user = SPContext.Current.Web.CurrentUser;
                    if (user == null)
                    {
                        ddlYears.Visible = false;
                        divPerformance.Visible = false;
                        divErr.InnerHtml = "您尚未登录，无法查看个人年度绩效汇总数据！";
                        btnPrint.Visible = false;
                        divQuery.Visible = false;
                    }
                    else
                    {
                        string IdCard = GetIdCard(user);
                        divQuery.Visible = true;
                        ddlYears.Visible = true;
                        GetPersonalPerformanceByYear(IdCard, webObj.ByYear);
                        //DisplayControl();
                    }
                }
            }
            catch (Exception ex)
            {

                divErr.InnerHtml = ex.ToString();
            }
        }

        /// <summary>
        /// 获取指定年度个人业绩汇总记录
        /// </summary>
        /// <param name="year">年度</param>
        /// <param name="IdCard">工号</param>
        /// <returns></returns>
        private void GetPersonalPerformanceByYear(string IdCard,string year)
        {
            string query = @"<Where><And><Eq><FieldRef Name='IDCard' /><Value Type='Text'>" + IdCard + "</Value></Eq><Eq><FieldRef Name='Year' /><Value Type='Text'>" + year + "</Value></Eq></And></Where>";
            SPListItemCollection items = GetDataFromList(webObj.ListName,query);
            if (items.Count > 0)
            {
                GetCollateDates();
                DisplayControl(IdCard);
                divPerformance.Visible = true;
                SPListItem item = items[0];
                hdId.Value = item.ID.ToString();
                hdIdCard.Value = item["工号"].ToString();
                //校对状态
                string flag = item["Flag"].ToString();

                checkFlag(flag);
                #region 个人资料
                lbName.Text = item["姓名"].ToString();
                lbIDCard.Text = item["工号"].ToString();
                lbProf.Text = item["职称"].ToString();
                lbDept.Text = item["所属部门"].ToString();
                #endregion

                #region 教学课时

                #region 本科教学课时
                lbgwjh_bk.Text = item["本科公外计划总学时"].ToString();
                lbzyjh_bk.Text = item["本科专业计划总学时"].ToString();
                lbgwzs_bk.Text = item["本科公外折算总学时"].ToString();
                lbzyzs_bk.Text = item["本科专业折算总学时"].ToString();
                lbzdshsj.Text = item["本科指导社会实践"].ToString();
                lbzdbylw_bk.Text = item["本科指导毕业论文"].ToString();
                lbxkk_bk.Text = item["本科新开课"].ToString();
                #endregion

                #region 研究生教学课时
                lbgwjh_yjs.Text = item["研究生公外计划总学时"].ToString();
                lbzyjh_yjs.Text = item["研究生专业计划总学时"].ToString();
                lbgwzs_yjs.Text = item["研究生公外折算总学时"].ToString();
                lbzyzs_yjs.Text = item["研究生专业折算总学时"].ToString();
                lbzdyjs.Text = item["指导研究生"].ToString();
                lbzdbylw_yjs.Text = item["研究生指导毕业论文"].ToString();
                lbyjspj.Text = item["研究生批卷"].ToString();
                #endregion

                #region 课时汇总
                lbgwjh.Text = item["公外计划总学时"].ToString();
                lbzyjh.Text = item["专业计划总学时"].ToString();
                lbjhw.Text = item["计划外总学时（公外+专业）"].ToString();


                #endregion

                #endregion


                #region 业绩点
                lbjxlx.Text = item["教学立项"].ToString();
                lbjc.Text = item["教材"].ToString();
                lblw.Text = item["论文"].ToString();
                lbyz.Text = item["译著"].ToString();
                lbzz.Text = item["专著"].ToString();
                lbkylx.Text = item["科研立项"].ToString();
                lbkycg.Text = item["科研成果"].ToString();
                lbjxhj.Text = item["教学获奖"].ToString();
                lbjxjs.Text = item["教学竞赛"].ToString();
                lbxzzw.Text = item["行政职务"].ToString();
                lbxkjs.Text = item["学科建设"].ToString();
                lbrcyj.Text = item["人才引进"].ToString();
                lbxsjz.Text = item["学术兼职"].ToString();
                lbjiafen.Text = item["加分"].ToString();
                lbjianfen.Text = item["减分"].ToString();

                #endregion


                //金额的字段
                DateTime[] dtjine = (DateTime[])ViewState["校对1"];
                if (isCheck(dtjine))
                {
                    lbOne.Text = "￥ " + item["人头费"].ToString();
                    lbkszje.Text = "￥ " + item["课时总金额"];
                    lbyjdz.Text = "￥ " + item["业绩点总金额"];
                    lbqueqin.Text = "￥ " + item["缺勤"];
                    lbzjbt.Text = "￥ " + item["助教补贴"];
                    lbyjdhjz.Text = "￥ " + item["业绩点合计总金额"];
                    lbzong.Text = "￥ " + item["总金额（人头+课时+业绩点合计）"];
                    lbyhb.Text = "￥ " + item["月核拨"];
                }
                divErr.InnerHtml = "";
                btnPrint.Visible = true;
            }
            else
            {
                divPerformance.Visible = false;
                divErr.InnerHtml = year + " 年度尚未有工号 <b>"+IdCard+"</b> 的业绩汇总记录！";
                btnPrint.Visible = false;
            }
        }

        /// <summary>
        /// 判断当前是否处于字符串指定的日期范围中
        /// </summary>
        /// <param name="dts">开始时间，截止时间的数组</param>
        /// <param name="user">指定用户</param>
        /// <returns></returns>
        private bool isCheck(DateTime[] dts)
        {
            bool ischeck = false;
            DateTime dtNow = DateTime.Now.Date;

            DateTime gpaStart = dts[0];//开始日期
            DateTime gpaEnd = dts[1].AddDays(1);//截止日期


            if (gpaStart <= dtNow & gpaEnd >= dtNow)//处于期间内
            {
                ischeck = true;
            }

            //若当前是管理员，且查看的不是本人的业绩汇总，则不显示校对确认按钮
            SPUser user = SPContext.Current.Web.CurrentUser;
            string idcard = GetIdCard(user);
            if (idcard !=hdIdCard.Value)
            {
                ischeck = false;
            }
            return ischeck;
        }

        private void GetCollateDates()
        {
            string query = @"<ViewFields>
                                  <FieldRef Name='Title' />
                                  <FieldRef Name='StartDate' />
                                  <FieldRef Name='EndDate' />
                               </ViewFields>
                               <Where>
                                  <Or>
                                     <Eq>
                                        <FieldRef Name='Title' />
                                        <Value Type='Text'>绩点校对</Value>
                                     </Eq>
                                     <Eq>
                                        <FieldRef Name='Title' />
                                        <Value Type='Text'>金额校对</Value>
                                     </Eq>
                                  </Or>
                               </Where>
                                <OrderBy>
                                      <FieldRef Name='StartDate' />
                                   </OrderBy>";
            string listName = webObj.CollateDate;
            SPListItemCollection collateDates = GetDataFromList(listName, query);
            DataTable dt = collateDates.GetDataTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime[] dates = new DateTime[] { (DateTime)(dt.Rows[i]["StartDate"]), (DateTime)(dt.Rows[i]["EndDate"])};
                ViewState["校对"+i.ToString()] = dates;
            }

        }

        /// <summary>
        /// 从SharePoint列表获取数据
        /// </summary>
        /// <param name="listName">列表名称</param>
        /// <param name="query">Caml查询语句</param>
        /// <returns></returns>
        private SPListItemCollection GetDataFromList(string listName, string query)
        {
            SPListItemCollection items = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                        {
                            SPList spList = spWeb.Lists.TryGetList(listName);

                            if (spList != null)
                            {
                                if (query != "")
                                {
                                    SPQuery qry = new SPQuery();
                                    qry.Query = query;
                                    items = spList.GetItems(qry);
                                }
                                else
                                {
                                    items = spList.GetItems();
                                }
                            }
                            else
                            {
                                divErr.InnerHtml = "指定的列表“" + listName + "”不存在！";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    divErr.InnerHtml= ex.ToString();
                }
            });
            return items;
        }

        private void checkFlag(string flag)
        {
            GetCollateDates();
            hdFlag.Value = flag;

            string yejiStatus = "<ul><li><b>当前业绩状态：</b>";
            switch (flag)
            {
                default:
                    DateTime[] dtjidian=(DateTime[])ViewState["校对0"];
                    yejiStatus+="业绩点数尚未校对，请及时检查校对！校对期限："+dtjidian[0]+"~"+dtjidian[1];
                    break;
                case "01":
                    yejiStatus+="业绩点数有异议，已申请修改，请及时与院办联系处理！";
                    btnGPANoPass.Enabled = false;
                    btnGPAPass.Enabled = true;
                    btnMoneyNoPass.Enabled = true;
                    btnMoneyPass.Enabled = true;
                    break;
                case "10":
                    yejiStatus+="业绩点数争议项已经与院办联系处理，等待您的确认！";
                    break;
                case "11":
                    yejiStatus+= "业绩点数无异议，已提交确认！";
                    btnGPANoPass.Enabled = true;
                    btnGPAPass.Enabled = false;
                    btnMoneyNoPass.Enabled = true;
                    btnMoneyPass.Enabled = true;
                    break;
                case "20":
                    DateTime[] dtjine=(DateTime[])ViewState["校对1"];
                    yejiStatus+="业绩金额数据已更新，请及时检查校对！校对期限："+dtjine[0]+"~"+dtjine[1];
                    break;
                case "02":
                    yejiStatus+="业绩金额有异议，已申请修改，请及时与院办联系处理！";
                    btnGPANoPass.Enabled = true;
                    btnGPAPass.Enabled = true;
                    btnMoneyNoPass.Enabled = false;
                    btnMoneyPass.Enabled = true;
                    break;
                case "12":
                    yejiStatus+= "业绩金额争议项已与院办联系处理完成，等待您的确认！";
                    break;
                case "22":
                    yejiStatus+= "业绩金额无异议，已提交确认，所有内容校对完毕√！";
                    btnGPANoPass.Enabled = true;
                    btnGPAPass.Enabled = true;
                    btnMoneyNoPass.Enabled = true;
                    btnMoneyPass.Enabled = false;
                    break;
            }
            pStatus.InnerHtml =yejiStatus+ "</li><li><b>特别提醒：</b>鼠标放置在当量值上时，可以查看该当量的计算规则</li>";
        }
        private void DisplayControl(string IdCard)
        {
            GetCollateDates();
            SPWeb web = SPContext.Current.Web;
            SPUser user = web.CurrentUser;
            if (user!=null)
            {
                #region 判断是否可查询
                string idcard = GetIdCard(user);
                if (webObj.Collater.Contains(idcard))
                {
                    lbID.Visible = true;
                    tbID.Visible = true;
                    btnQuery.Visible = true;

                }
                else
                {
                    lbID.Visible = false;
                    tbID.Visible = false;
                    btnQuery.Visible = false;
                }
                #endregion

                #region 判断业绩校对时期
                DateTime[] dtjidian = (DateTime[])ViewState["校对0"];
                if (isCheck(dtjidian))
                {
                    GPACheck.Visible = true;
                }
                else
                {
                    GPACheck.Visible = false;
                }

                DateTime[] dtjine = (DateTime[])ViewState["校对1"];
                if (isCheck(dtjine))
                {
                    MoneyCheck.Visible = true;
                }
                else
                {
                    MoneyCheck.Visible = false;
                }
                #endregion

            }

            else
            {
                lbID.Visible = false;
                tbID.Visible = false;
                btnQuery.Visible = false;
                GPACheck.Visible = false;
                MoneyCheck.Visible = false;
            }
        }

        private void BindYears()
        {
            int thisYear = DateTime.Now.Year;
            for (int i = 0; i < 10; i++)
            {
                ListItem item = new ListItem
                {
                    Value = (thisYear-i).ToString(),
                    Text = (thisYear-i).ToString()
                };
                ddlYears.Items.Add(item);
            }
            ddlYears.SelectedIndex=0;
        }

        protected void ddlYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            string year = ddlYears.SelectedValue;
            SPUser user = SPContext.Current.Web.CurrentUser;
            string IdCard = GetIdCard(user);
            if (!string.IsNullOrEmpty(tbID.Text.Trim()))
            {
                IdCard = tbID.Text.Trim();
            }
            GetPersonalPerformanceByYear(IdCard,year);

        }

        /// <summary>
        /// 获取指定用户的登录名
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GetIdCard(SPUser user)
        {
            string idCard = "";
            if (user != null)
            {
                idCard = user.LoginName;
                if (idCard.Contains("|"))
                {
                    idCard = idCard.Substring(idCard.IndexOf("|") + 1);
                }
                if (idCard.Contains("\\"))
                {
                    idCard = idCard.Substring(idCard.IndexOf("\\") + 1);
                }
            }
            return idCard;
        }

        /// <summary>
        /// 判断用户是否属于指定用户组
        /// </summary>
        /// <param name="groupName">用户组名称</param>
        /// <param name="user">指定SharePoint用户</param>
        /// <returns></returns>
        private bool GetGroupofUser(SPUser user,string groupName)
        {
            bool flag = false;
            using (SPSite siteCollection = SPContext.Current.Site)
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    //获取该用户在site/web中所有的组
                    SPGroupCollection userGroups = user.Groups;
                    //循环判断当前用户所在的组有没有给定的组
                    foreach (SPGroup group in userGroups)
                    {
                        //Checking the group
                        if (group.Name.Contains(groupName))
                            flag = true;
                        break;
                    }
                }
            }

            return flag;
        }


        protected void btnQuery_Click(object sender, EventArgs e)
        {
            string year = DateTime.Now.Year.ToString();
            SPUser user = SPContext.Current.Web.CurrentUser;
            string IdCard = GetIdCard(user);
            DisplayControl(IdCard);
            if (!string.IsNullOrEmpty(ddlYears.SelectedValue)&&!string.IsNullOrEmpty(tbID.Text.Trim()))
            {
                year = ddlYears.SelectedValue;
                IdCard = tbID.Text.Trim();
                GetPersonalPerformanceByYear(IdCard, year);
            }
            else
            {
                divErr.InnerHtml = "请选择要查询的年份,并输入查询的工号";
            }
        }



        private void changeStatus(string flag)
        {
            string Id = "";
            string siteUrl = SPContext.Current.Site.Url;
            if (!string.IsNullOrEmpty(webObj.WebUrl))
            {
                siteUrl = webObj.WebUrl;
            }
            try
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        spWeb.AllowUnsafeUpdates = true;
                        SPList spList = spWeb.Lists.TryGetList(webObj.ListName);
                        if (spList != null)
                        {
                            Id = hdId.Value;
                            int id = int.Parse(Id);
                            SPListItem item = spList.GetItemById(id);
                            item["Flag"] = flag;
                            item.Update();
                        }
                        spWeb.AllowUnsafeUpdates = false;
                        checkFlag(flag);
                    }
                }
            }
            catch (Exception ex)
            {

                divErr.InnerHtml="状态变更错误：ID="+Id+";"+ex.ToString();
            }

        }

        #region 按钮事件
        //汇总状态标记值与状态
        //00：业绩点未校对；
        //01：业绩点有误，已提交申请，等待院办修改；
        //10：绩点数错误院办已处理，等待教师二次确认；
        //11：业绩绩点数无异议，已确认，等待业绩金额更新中；
        //20：业绩二次导入，金额数已更新，等待金额确认
        //02：业绩金额数有异议，等待院办处理；
        //12：业绩金额争议院办已处理，等待教师确认；
        //22：业绩金额无异议，本年度业绩数据校对完成

        /// <summary>
        /// 业绩点数无异议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGPAPass_Click(object sender, EventArgs e)
        {
            changeStatus("11");
            divErr.InnerHtml = "业绩点数无异议，该项校对完毕，等待公布业绩金额。";
        }


        /// <summary>
        /// 业绩点数有异议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGPANoPass_Click(object sender, EventArgs e)
        {
            changeStatus("01");
            divErr.InnerHtml = "业绩点数异议已发出，请与院办及时联系处理。";
        }

        /// <summary>
        /// 业绩金额无异议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnMoneyPass_Click(object sender, EventArgs e)
        {
            changeStatus("22");
            divErr.InnerHtml = "业绩金额无异议，业绩金额校对完毕。";
        }

        /// <summary>
        /// 业绩金额有异议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnMoneyNoPass_Click(object sender, EventArgs e)
        {
            changeStatus("02");
            divErr.InnerHtml = "业绩金额异议已提交，请与院办及时联系处理。";
        }
        #endregion


    }
}
