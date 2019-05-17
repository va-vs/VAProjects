using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;

namespace CreativeAppraise.Appraise
{
    public partial class AppraiseUserControl : UserControl
    {
        #region 事件
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack )
            {
                if (SPContext.Current.Web.CurrentUser != null)
                {
                    string strStep = GetAppraiseAction();
                    if (strStep == "")
                    {
                        lbAppraise.Text = "当前未处于评审阶段!";
                        AppAction.Visible = false;
                    }
                    else
                    {
                        AppAction.Visible = true;
                        lbAppraise.Text = strStep;
                        action = strStep;
                        int cNum = 0;
                        creatity = GetCreativityName(ref cNum);
                        lbContentNum.Text = lbContentNum.Text.Replace("N", cNum.ToString());
                        lbContentNum.Text = lbContentNum.Text.Replace("创意",webObj.ListName);
                        string ID = Page.Request.QueryString["ID"];
                        SPListItem item = GetAppraiseRecord(action, ID );
                        this.txtAppraise.ToolTip = this.txtAppraise.ToolTip.Replace("N", webObj.AppraiseNum.ToString());
                        this.txtScore.ToolTip = this.txtScore.ToolTip.Replace("N", webObj.Score.ToString());
                        lbACommnts.Text = lbACommnts.Text.Replace("N", webObj.AppraiseNum.ToString());
                        string scoreName = GetScoreDispName;
                        if (item != null)
                        {
                            //btnAppraise.Enabled = false;
                            if (item["评语"] != null)
                                txtAppraise.Text = item["评语"].ToString();
                            if (item[scoreName] != null)
                                txtScore.Text = item[scoreName].ToString();
                            //ddlResult.SelectedItem.Text = item["结果"].ToString(); 
                        }
                    }
                    
                }
                else
                {
                    AppraiseDiv.Visible = false;
                    
                }

            }
            //setLabels();
            btnAppraise.Click += btnAppraise_Click;
            btnCancle.Click += btnCancle_Click;
        }
       
        void btnCancle_Click(object sender, EventArgs e)
        {
            Response.Redirect(webObj.RetUrl);
        }

        void btnAppraise_Click(object sender, EventArgs e)
        {
            bool result;
            result = ValidData();
            if (!result) return;
            result=AddAppraiseRecord(action, creatity);
            if (!result)
                lblMsg.Text = "评审失败";
            else
            {
                string ID = Page.Request.QueryString["ID"];
                float score = CaculateAppraiseScore(action, ID);
                WriteCreaScore(score);
                Response.Redirect(webObj.RetUrl );
            }
 
        }
        #endregion
        #region 方法
        /// <summary>
        /// 设置表单中的标签
        /// </summary>
        public void setLabels()
        {
            string ID = Page.Request.QueryString["ID"];
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(webObj.ResultList);
            if (list != null)
            {
                SPFieldCollection spFields = list.Fields;
                lbScore.Text = list.Fields[0].ToString();
                
                    
            }
        }

        /// <summary>
        /// 获取当前要的创意名称(Title),并统计字数，正文或项目简介（创意简介）
        /// modified at 2018-05-03
        /// </summary>
        /// <returns></returns>
        private string GetCreativityName(ref int contentNum)
        {
            string ID=Page.Request.QueryString ["ID"];
            SPWeb web=SPContext.Current.Web;
            string lstName=webObj.ListName;
            SPList list=web.Lists.TryGetList ( lstName );
            string strContent="";
            if (list!=null)
            {
                SPListItem itme=list.GetItemById (int.Parse (ID ));
                if (itme !=null)
                {   if (list.Fields.ContainsField (webObj.BodyField))
                        strContent=itme[webObj.BodyField] !=null?itme[webObj.BodyField].ToString ():"";
                    contentNum = strContent.Length == 0 ? 0 : GetHanNumFromString(strContent);
                    //统计字段（正文或描述，此字段名称为参数）  
                    return itme[ list.Fields.GetFieldByInternalName("Title").Title  ].ToString (); 
                }  

            }
            return "";
        }
        //加入计算平均分
        public bool AddAppraiseRecord(string strAction, string strCreativity)
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);

                        if (spList != null)
                        {
                            spWeb.AllowUnsafeUpdates = true;
                            SPListItem listItem;
                            if (ViewState["id"] == null)
                            {
                                string ID = Page.Request.QueryString["ID"];
                                listItem = spList.AddItem();
                                listItem[webObj.ListName +"名称"] = ID + ";#" + strCreativity;
                                listItem[GetAuthorDispName ] = appraiseUser.ID + ";#" + appraiseUser.Name;//"创建者"
                                listItem["动作"] = strAction;//动作
                                //listItem["作品状态"] = strAction;
                            }

                            else
                            {
                                listItem = spList.Items.GetItemById((int)ViewState["id"]);
                            }
                            listItem["时间"] = DateTime.Now; 
                            listItem["评语"] = txtAppraise.Text;
                            //listItem["结果"] = ddlResult.SelectedItem.Text;
                            listItem[GetScoreDispName ] = float.Parse(txtScore.Text);//分数
                            listItem.Update();

                            spWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
            return true;
        }
        private bool WriteCreaScore(float score)
        {
            string ID = Page.Request.QueryString["ID"];
            string siteUrl = SPContext.Current.Site.Url;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb web = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList list = web.Lists.TryGetList(webObj.ListName);
                        if (list != null)
                        {
                            SPListItem item = list.GetItemById(int.Parse(ID));
                            SPModerationStatusType appraiseState;
                            if (item != null)
                            {
                                try
                                {
                                    web.AllowUnsafeUpdates = true;
                                    item[GetScoreDispName] = score;
                                    appraiseState = item.ModerationInformation.Status;
                                    item.SystemUpdate();
                                    item.ModerationInformation.Status = appraiseState;
                                    item.SystemUpdate();
                                    web.AllowUnsafeUpdates = false;
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
            });
            return true;
        }
        //计算创意的平均分数，保存当前创意评审以后，2017-11-6
        public float  CaculateAppraiseScore(string strAction, string strCreativityID)
        {
            string siteUrl = SPContext.Current.Site.Url;
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);
                    if (spList != null)
                    {
                        string fldAction="操作";
                        string fldRelation=webObj.ListName +"名称";//查阅项字段中文名称，与主表的中文名
                        fldAction = spList.Fields.GetField(fldAction).InternalName;
                        fldRelation = spList.Fields.GetField(fldRelation).InternalName; 
                        SPQuery qry = new SPQuery();
                        qry.Query = @"<Where><And><Eq><FieldRef Name='"+fldAction +"' /><Value Type='Text'>" + strAction + "</Value></Eq><Eq><FieldRef Name='"+fldRelation +"' LookupId='True' /><Value Type='Lookup'>" + strCreativityID + "</Value></Eq></And></Where>";
                        SPListItemCollection listItems = spList.GetItems(qry);
                        float score = 0;
                        int cTotal = 0;
                        foreach (SPListItem oListItem in listItems)
                        {
                            if (oListItem[GetScoreDispName ] != null)
                            {
                                score = score + float.Parse(oListItem[GetScoreDispName ].ToString());
                                cTotal = cTotal + 1;
                            }
                        }
                        if (score > 0)
                            score = score / cTotal;
                        return (float)Math.Round(score, 1);
                    }
                }
                return 0;
            }
        }
        //是否已经评审
        /// <summary>
        /// 当前登录者是否已经审批过，只能审批一次
        /// </summary>
        /// <param name="strAction">操作名称</param>
        /// <param name="strCreativity">创意名称</param>
        /// <returns></returns>
        public SPListItem   GetAppraiseRecord(string strAction,string strCreativityID)
        {
            string siteUrl = SPContext.Current.Site.Url;
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID ))
                {
                    SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);
                    if (spList != null)
                    {
                        SPQuery qry = new SPQuery();
                        qry.Query = @"<Where><And><And><Eq><FieldRef Name='Author' /> <Value Type='Integer'> <UserID /> </Value></Eq><Eq><FieldRef Name='Title' /><Value Type='Text'>" + strAction + "</Value></Eq></And><Eq><FieldRef Name='creativity' LookupId='True' /><Value Type='Lookup'>" + strCreativityID + "</Value></Eq></And></Where>";
            
                        SPListItemCollection listItems = spList.GetItems(qry);
                        if (listItems.Count > 0)
                        {
                            ViewState["id"]= listItems[0]["ID"];
                            return listItems[0] ;
                        }
                    }
                }
            }

            return null;
        }
     
        /// <summary>
        /// 日程 表，进行到什么阶段
        /// </summary>
        /// <returns></returns>
        //根据当前时间判断当前进行到什么审批阶段，如果为空则当前没有进行审批
        public string GetAppraiseAction()
        {
            string siteUrl = SPContext.Current.Site.Url;
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID ))
                {
                    SPList spList = spWeb.Lists.TryGetList(webObj.TimeList);
                    if (spList != null)
                    {
                        SPQuery qry = new SPQuery();
                        qry.Query = @"<Where><And>
                                         <Leq>
                                            <FieldRef Name='StartDate' />
                                            <Value Type='DateTime'>
                                               <Today />
                                            </Value>
                                         </Leq>
                                         <Gt>
                                            <FieldRef Name='EndDate' />
                                            <Value Type='DateTime'>
                                               <Today />
                                            </Value>
                                         </Gt>
                                      </And></Where>";

                        SPListItemCollection listItems = spList.GetItems(qry);
                        if (listItems.Count > 0)
                        {
                            string dispTitle = spList.Fields.GetFieldByInternalName("Title").Title;
                            return listItems[0][dispTitle ].ToString ();//"标题"
                        }

                    }
                }
            }
            return "";
        }
        #endregion
        #region 方法
        /// <summary>
        /// 统计中文汉字的个数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetHanNumFromString(string str)
        {
            int count = 0;
            Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (regex.IsMatch(c.ToString()))
                {
                    count++;
                }
            }
            return count;
        }
        #endregion
        #region 获取字段显示名的属性
        //获取分数的中文名
        private string GetScoreDispName
        {
            get
            {
                if (ViewState["ScoreShowName"] != null)
                    return ViewState["ScoreShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.ResultList);
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("Score");
                        ViewState["ScoreShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
        }
        //创建者
        private string GetAuthorDispName
        {
            get
            {
                if (ViewState["AuthorShowName"] != null)
                    return ViewState["AuthorShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.ResultList);
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("Author");
                        ViewState["AuthorShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
        }
        #endregion
        #region 属性
        public Appraise webObj { get; set; }
        string _action;
        public string action
        {
            get {
                if (_action == null)
                    _action = GetAppraiseAction();
                return _action; }
            set { _action = value; }
        }
        string _creativty;
        public  string creatity 
        {
            get {
                int cNum=0;
                if (_creativty == null)
                    _creativty = GetCreativityName(ref cNum);
                return _creativty; }
            set { _creativty = value; }
        }
        #endregion
        #region 数据验证
        /// <summary>
        /// 判断评语是否不少于100字,将评语和分数写在参数中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected bool ValidData()
        {
            bool validResult = false;
            // 判断评分是否是0-10之间的数字
            string strScore = txtScore.Text;
            if (string.IsNullOrEmpty(strScore))
            {
                lbAScore.Text = "评分不能为空!";
                lbAScore.Visible = true;
                //btnAppraise.Enabled = false;
                validResult = false;
            }
            else
            {
                if (IsNumeric(strScore))
                {
                    if (Convert.ToSingle(strScore) < 0 || Convert.ToSingle(strScore) > webObj.Score )
                    {
                        lbAScore.Text = "评分必须在0-" + webObj.Score.ToString() + "之间!";
                        lbAScore.Visible = true;
                        validResult = false;
                    }
                    else
                    {
                        lbAScore.Visible = false;
                        validResult = true;
                    }
                }
                else
                {
                    lbAScore.Text = "评分必须是0-"+webObj.Score.ToString ()+"之间的数字!";
                    lbAScore.Visible = true;
                    validResult = false;
                }
            }
            if (validResult)
            {
                if (txtAppraise.Text.Length < webObj.AppraiseNum)
                {
                    lbACommnts.Visible = true;
                    return false;
                }
                else
                {
                    //btnAppraise.Enabled = true;
                    lbACommnts.Visible = false;
                    return true;
                }
            }
            else
                return false;
        }

        public static bool IsNumeric(string value)
        {
            //Regex.IsMatch(value, @"^\d+$");
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?/d*$");
        }
        public static bool IsUnsign(string value)
        {
            return Regex.IsMatch(value, @"^/d*[.]?/d*$");
        }
        #endregion
    }
}
