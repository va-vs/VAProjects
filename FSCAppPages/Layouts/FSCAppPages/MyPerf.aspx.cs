using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Configuration;

namespace FSCAppPages.Layouts.FSCAppPages
{
    public partial class MyPerf : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindYears();

                string IdCard = GetIdCard();
                if (IdCard == "")
                {
                    ddlYears.Visible = false;
                    divPerformance.Visible = false;
                    divErr.InnerHtml = "您尚未登录，无法查看个人年度绩效汇总数据！";
                    btnPrint.Visible = false;
                    divQuery.Visible = false;
                }
                else
                {
                    divQuery.Visible = true;
                    ddlYears.Visible = true;
                    divPerformance.Visible = true;
                    GetPersonalPerformanceByYear(IdCard, ConfigurationManager.AppSettings["ByYear"]);
                }
            }
            DisplayControl();
        }

        /// <summary>
        /// 获取指定年度个人业绩汇总记录
        /// </summary>
        /// <param name="year">年度</param>
        /// <param name="IdCard">工号</param>
        /// <returns></returns>
        private void GetPersonalPerformanceByYear(string IdCard,string year)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
           {
               try
               {
                    string siteUrl = SPContext.Current.Site.Url;
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["pfWebUrl"]))
                    {
                        siteUrl = ConfigurationManager.AppSettings["pfWebUrl"];
                    }
                    using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            SPList spList = spWeb.Lists.TryGetList(ConfigurationManager.AppSettings["ListName"]);
                            if (spList != null)
                            {
                                lbTitle.Text = year + " 年度个人业绩汇总";

                                SPQuery qry = new SPQuery
                                {
                                    RowLimit = 1,
                                    Query = @"<Where><And><Eq><FieldRef Name='IDCard' /><Value Type='Text'>" + IdCard + "</Value></Eq><Eq><FieldRef Name='Year' /><Value Type='Text'>" + year + "</Value></Eq></And></Where>"
                                };
                                SPListItemCollection items = spList.GetItems(qry);
                                if (items.Count > 0)
                                {
                                    SPListItem item = items[0];
                                    hdId.Value = item.ID.ToString();
                                    hdIdCard.Value = item["工号"].ToString();
                                    DisplayControl();
                                    divPerformance.Visible = true;
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
                                    lbjhw.Text = item["计划外总学时"].ToString();


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
                                    if (isCheck(ConfigurationManager.AppSettings["fscMoneyDate"]))
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
                            else
                            {
                                divErr.InnerHtml = "尚未公布业绩汇总记录，请以后再试！";
                                btnPrint.Visible = false;
                            }
                        }

                   }
               }
               catch (Exception ex)
               {
                   divErr.Visible = true;
                   divErr.InnerHtml = ex.ToString();
                   btnPrint.Visible = false;
               }
           });
        }

        /// <summary>
        /// 判断当前是否处于字符串指定的日期范围中
        /// </summary>
        /// <param name="periodStr">事件时间范围字符串，格式为：起始日期;截止日期，如:2018/10/01;2018/10/30</param>
        /// <returns></returns>
        private bool isCheck(string periodStr)
        {
            bool ischeck = false;
            DateTime dtNow = DateTime.Now.Date;
            string[] gpas =periodStr.Split(';');

            DateTime gpaStart = DateTime.Parse(gpas[0]);//开始日期
            DateTime gpaEnd = DateTime.Parse(gpas[1]);//截止日期


            if (gpaStart <= dtNow & gpaEnd >= dtNow)//处于期间内
            {
                ischeck = true;
            }

            //若当前是管理员，且查看的不是本人的业绩汇总，则不显示校对确认按钮
            string idcard = GetIdCard();
            if (idcard !=hdIdCard.Value)
            {
                ischeck = false;
            }
            return ischeck;
        }
        private void checkFlag(string flag)
        {
            hdFlag.Value = flag;

            string yejiStatus = "<ul><li><b>当前业绩状态：</b>";
            switch (flag)
            {
                default:
                    string[] gpas = ConfigurationManager.AppSettings["fscGPADate"].Split(';');
                    yejiStatus+="业绩点数尚未校对，请及时检查校对！校对期限："+gpas[0]+"~"+gpas[1];
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
                    string[] moneys = ConfigurationManager.AppSettings["fscMoneyDate"].Split(';');
                    yejiStatus+="业绩金额数据已更新，请及时检查校对！校对期限："+moneys[0]+"~"+moneys[1];
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
            pStatus.InnerHtml =yejiStatus+ "</li><li><b>特别提醒：</b>点击业绩当量值后面的 ❉ 号，可以查看计算规则与说明</li>";
        }
        private void DisplayControl()
        {
            SPWeb web = SPContext.Current.Web;
            SPUser user = web.CurrentUser;
            if (user!=null)
            {
                #region 判断是否可查询

                if (GetGroupofUser("业绩校对"))
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

                if (isCheck(ConfigurationManager.AppSettings["fscMoneyDate"]))
                {
                    GPACheck.Visible = true;
                }
                else
                {
                    GPACheck.Visible = false;
                }


                if (isCheck(ConfigurationManager.AppSettings["fscMoneyDate"]))
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
            string IdCard = GetIdCard();
            if (!string.IsNullOrEmpty(tbID.Text.Trim()))
            {
                IdCard = tbID.Text.Trim();
            }
            GetPersonalPerformanceByYear(IdCard,year);

        }

        private string GetIdCard()
        {
            string idCard = "";
            SPWeb web = SPContext.Current.Web;
            SPUser user = web.CurrentUser;

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

        private bool GetGroupofUser(string groupName)
        {
            bool flag = false;
            using (SPSite siteCollection = SPContext.Current.Site)
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    //string groupName = "TestGroup";
                    //获取当前登录的用户
                    SPUser currentUser = site.CurrentUser;

                    //获取该用户在site/web中所有的组
                    SPGroupCollection userGroups = currentUser.Groups;
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
            string IdCard = GetIdCard();
            if (!string.IsNullOrEmpty(ddlYears.SelectedValue)&&!string.IsNullOrEmpty(tbID.Text.Trim()))
            {
                year = ddlYears.SelectedValue;
                IdCard = tbID.Text.Trim();
                GetPersonalPerformanceByYear(IdCard, year);
            }
            else
            {
                divErr.InnerHtml = "请选择要查询的年份和工号";
            }
        }



        private void changeStatus(string flag)
        {
            string Id = "";
            string siteUrl = SPContext.Current.Site.Url;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebUrl"]))
            {
                siteUrl = ConfigurationManager.AppSettings["WebUrl"];
            }
            try
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        spWeb.AllowUnsafeUpdates = true;
                        SPList spList = spWeb.Lists.TryGetList(ConfigurationManager.AppSettings["ListName"]);
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
