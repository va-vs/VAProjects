using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.Collections.Generic;

namespace PerformanceWrite.Write
{
    public partial class WriteUserControl : UserControl
    {
        #region 事件
        public Write webObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

            gvGoalSetting.RowCommand += GvGoalSetting_RowCommand;
            gvGoalSetting.RowDataBound += GvGoalSetting_RowDataBound;
            gvGoalSetting.RowDeleting += GvGoalSetting_RowDeleting;
            if (!Page.IsPostBack)
            {
                SPWeb web = SPContext.Current.Web;
                //SPUser user = web.CurrentUser;
                try
                {
                    //lblName.Text = GetAuthorDispName+"：";
                    //lblRatio.Text = GetRadioDispName+"：";
                    //lblAuthors.Text = webObj.AuthorListTitle;//已经分配业绩
                    lbAppraise.Text = webObj.ShowTitle;
                    //InitAuthors();
                    InitGridView();
                }
                catch (Exception ex)
                {
                    lblMsg.Text = ex.ToString();
                }
            }
            btnSave.Click += btnSave_Click;
            btnNew.Click += btnNew_Click;
            //btnRet.Click += btnRet_Click;
            //rblAuthors.SelectedIndexChanged += rblAuthors_SelectedIndexChanged;
        }

        private void GvGoalSetting_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        private void GvGoalSetting_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int id = int.Parse(gvGoalSetting.DataKeys[e.Row.RowIndex].Value.ToString());
                IDictionary<int, string> authors = (Dictionary<int, string>)ViewState["Authors"];
                string lgName;
                authors.TryGetValue(id, out lgName);

                PeopleEditor tbName1 = (PeopleEditor)e.Row.Cells[0].FindControl("tbName1");//获取 模板列的值
                tbName1.CommaSeparatedAccounts = lgName;
                //PickerEntity picker in tbName.ResolvedEntities.cou

            }
        }

        private void GvGoalSetting_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                int Id = int.Parse(e.CommandArgument.ToString());
                GridViewRow drv = ((GridViewRow)(((LinkButton)(e.CommandSource)).Parent.Parent)); //此得出的值是表示那行被选中的索引值
                DataTable dt = (DataTable)ViewState["gvSource"];
                if (Id > 0)
                {
                    bool r = DeletePerformRecord(Id);
                 }
                DataRow dr = dt.Select("ID="+Id )[0];
                dr.Delete();
                ViewState["gvSource"] = dt;
                gvGoalSetting.DataSource = dt.DefaultView;
                gvGoalSetting.DataBind();
                lblMsg.Text = "";

            }
        }

        //返回到指定的页面改为删除，当前的用户业绩，或者为零
        void btnRet_Click(object sender, EventArgs e)
        {
            //Response.Redirect(webObj.RetUrl);
            if (ViewState["id"]==null )
            {
                lblMsg.Text = "请先选择要删除的业绩分配项";
                return;
            }
            //bool result= DeletePerform();
            //if (result)
            //{
            //    rblAuthors.Items.Remove(rblAuthors.SelectedItem);
            //    tbRatio.Text = "";
            //    tbName.CommaSeparatedAccounts = "";

            //}

        }
        //切换各个用户
        void rblAuthors_SelectedIndexChanged(object sender, EventArgs e)
        {
            string item = rblAuthors.SelectedValue;//userID+listID+ratio
            string[] ids=Regex.Split ( item,";");
            ViewState["id"] = ids[1];//业绩ID
            ViewState["beforeRatio"] =double.Parse ( ids[2]);//业绩修改前的值
            SPWeb web=SPContext.Current.Web;
            SPFieldUserValue user = new SPFieldUserValue(web, int.Parse(ids[0]), rblAuthors.SelectedItem.Text);
            tbName.CommaSeparatedAccounts = user.User.LoginName;
            tbRatio.Text = ids[2];
            btnSave.Enabled = true;
            lblMsg.Text = "";
         }
        private void setFocus()
        {
            tbRatio.Focus();

            tbRatio.Attributes.Add("onfocus", "this.select()");
        }


        //清空
        void btnNew_Click(object sender, EventArgs e)
        {
            List<string> results = new List<string>();
           if (CheckRatio (ref results,"New"))
            {
                DataTable dt = (DataTable)ViewState["gvSource"];//  gvGoalSetting.DataSource;
                DataRow dr = dt.NewRow();
                int id = -1;
                if (gvGoalSetting.Rows.Count >0)
                    id=(int)gvGoalSetting.DataKeys[gvGoalSetting.Rows.Count - 1].Value;
                dr["ID"] = id > 0 ? -1 : id - 1;
                if (ViewState["TotalRatio"] == null)
                    dr["Ratio"] = TotalRatio ;
                else
                    dr["Ratio"] =  TotalRatio - (decimal)ViewState["TotalRatio"] ;
                dt.Rows.Add(dr);

                DataBind(dt);
            }
        }
        /// <summary>
        /// 新建和保存时进行判断,
        /// 保存时，业绩点之和不能大于1；新建时，如果业绩点之点等于1，则不能创建新的业绩点
        /// </summary>
        /// <param name="results"></param>
        /// <param name="btnEvent">Save/New</param>
        /// <returns></returns>
        private bool CheckRatio( ref List<string> results ,string btnEvent="Save")
        {
            lblMsg.Text = "";
            decimal   tRatio = 0;

            SPWeb web = SPContext.Current.Web;
            List<int> authors = new List<int>();
            List<string> result = new List<string>();
            for (int i = 0; i < gvGoalSetting.Rows.Count; i++)
            {
                TextBox tbRatio = (TextBox)gvGoalSetting.Rows[i].Cells[1].FindControl("tbRatio1");//获取 模板列的值
                if (tbRatio.Text != "")
                    tRatio += decimal .Parse(tbRatio.Text);

                PeopleEditor tbName1 = (PeopleEditor)gvGoalSetting.Rows[i].Cells[0].FindControl("tbName1");//获取 模板列的值

                if (tbName1.ResolvedEntities.Count > 0)
                {
                    SPFieldUserValue user = GetUserValue (web,(PickerEntity)tbName1.ResolvedEntities[0]);
                    if (authors.Contains(user.User.ID))
                    {
                        lblMsg.Text = webObj.ShowMsgAuthor;
                        return false;
                    }
                    else
                        authors.Add(user.LookupId);
                    result.Add(gvGoalSetting.DataKeys[i].Value.ToString() + ";" +user.User.ID + ";" + (tbRatio.Text == "" ? "0" : tbRatio.Text));
                }
            }
            tRatio = decimal.Round(tRatio, 2);
            ViewState["TotalRatio"] = tRatio;//所有已经输入的业绩
            if (btnEvent=="Save" && tRatio > TotalRatio ||btnEvent=="New" && tRatio == TotalRatio)
            {
                lblMsg.Text = webObj.ShowMsg.Replace("N", (TotalRatio - tRatio).ToString());
                return false;
            }

            results = result;
            return true ;
        }


        //保存，判断个数
        void btnSave_Click(object sender, EventArgs e)
        {
            List<string> results = new List<string>();
            if (CheckRatio(ref results))
            {

                UpdateAllPerforms(results);
                lblMsg.Text = "保存成功！";
            }
        }


        #endregion
        #region  初始化控件
        private void InitGridView()
        {
            string ID = Page.Request.QueryString["ID"];//论文ID
                                                       //读取已有的业绩分配
            DataTable dt = GetPerformRecord(webObj.CurrentAction, ID);//获取当前论文的业绩
            DataBind(dt);

        }
        private void DataBind(DataTable dt)
        {


            gvGoalSetting.DataSource = dt.DefaultView;
            gvGoalSetting.DataBind();
            if (dt.Rows.Count == 0)
                btnSave.Enabled = false;
            else
                btnSave.Enabled = true;
        }

        #endregion
        #region 方法
        private SPFieldUserValueCollection GetPaperAuthorName()
        {
            string ID = Page.Request.QueryString["ID"];
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(webObj.FromList);
            if (list != null)
            {
                SPListItem itme = list.GetItemById(int.Parse(ID));
                if (itme != null && itme[GetAuthorNameDispName] != null)
                {
                    SPFieldUserValueCollection authors = itme[GetAuthorNameDispName] as SPFieldUserValueCollection;
                    return authors;
                }
            }
            return null;
        }
        //获取系数的中文名称
        private string GetRadioDispName
        {
            get
            {
                if (ViewState["RadioShowName"] != null)
                    return ViewState["RadioShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.ResultList);
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("Ratio");
                        ViewState["RadioShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
        }

        //创建者业绩分配者，即为当前用户
        private string GetAuthorDispName
        {
            get
            {
                if (ViewState["AuthorShowName"] != null)
                    return ViewState["AuthorShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.ResultList );
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("AuthorName");
                        ViewState["AuthorShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
        }

        //获取主表中的总的系数和,总系数之和不能大于1
        decimal   _totalRatio=0;
        public decimal   TotalRatio
        {
            get
            {
                if (_totalRatio == 0)
                    _totalRatio = 1;// GetRatio ();
                return _totalRatio;
            }
            set { _totalRatio = value; }
        }
        /// <summary>
        /// 获取院内作者的显示名
        /// </summary>
        private string GetAuthorNameDispName
        {
            get
            {
                if (ViewState["AuthorNameShowName"] != null)
                    return ViewState["AuthorNameShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;

                    SPList list = web.Lists.TryGetList(webObj.FromList);
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("AuthorName");
                        ViewState["AuthorNameShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
        }
        /// <summary>
        /// 主表中作者的现实名，用分号分开的字符串
        /// </summary>
        private string GetAuthorsDispName
        {
            get
            {
                if (ViewState["AuthorsShowName"] != null)
                    return ViewState["AuthorsShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.FromList);
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("Authors");
                        ViewState["AuthorsShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
        }
        //获到主表标题的中文显示名称，如论文标题，内部名称为Title
        private string GetTitleDispName
        {
            get
            {
                if (ViewState["TitleShowName"] != null)
                    return ViewState["TitleShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.FromList);
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("Title");
                        ViewState["TitleShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
         }
        //删除分配项
        //private bool DeletePerform()
        //{
        //    string siteUrl = SPContext.Current.Site.Url;
        //    bool ret = false;
        //    SPSecurity.RunWithElevatedPrivileges(delegate ()
        //    {
        //        using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
        //        {
        //            using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
        //            {
        //                SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);

        //                if (spList != null)
        //                {
        //                    spWeb.AllowUnsafeUpdates = true;
        //                    SPListItem listItem;

        //                    int id = int.Parse(ViewState["id"].ToString());
        //                    listItem = spList.Items.GetItemById(id);
        //                    listItem.Delete();
        //                    spWeb.AllowUnsafeUpdates = false;
        //                    ret= true;
        //                }
        //            }
        //        }
        //    });
        //    return ret;
        //}
        private void UpdateAllPerforms(List<string> performs )
        {
            SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID )) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);

                        if (spList != null)
                        {
                            SPQuery query;
                            SPListItemCollection items;
                            string ID = Page.Request.QueryString["ID"];
                            spWeb.AllowUnsafeUpdates = true;
                            SPListItem listItem;
                            string lookupInternalName = GetSubTitleInterlname.Trim();
                            foreach (string strPerform in performs)
                            {
                                string[] itmes = strPerform.Split(';');
                                int id = int.Parse(itmes[0]);
                                if (id < 0)
                                {
                                    query = new SPQuery();
                                    query.Query = @"<Where><And><And><Eq><FieldRef Name='AuthorName' LookupId='True' /><Value Type='Integer'>" + itmes[1] + "</Value></Eq><Eq><FieldRef Name='Action' /><Value Type='Choice'>" + webObj.CurrentAction + "</Value></Eq></And><Eq><FieldRef Name='" + lookupInternalName + "' LookupId='True' /><Value Type='Lookup'>" + ID + "</Value></Eq></And></Where>";
                                    items = spList.GetItems(query);
                                    if (items.Count == 0)
                                    {
                                        listItem = spList.AddItem();
                                        listItem[GetTitleDispName] = ID;// + ";#" + strPerformance;
                                    }
                                    else
                                        continue;
                                }

                                else
                                {

                                    listItem = spList.Items.GetItemById(id);
                                }
                                listItem[GetAuthorDispName] = int.Parse(itmes[1]);  //GetUserValue(spWeb, picker);//用户填写的
                                listItem[GetRadioDispName] = double.Parse(itmes[2]);//系数
                                listItem["动作"] = webObj.CurrentAction;
                                listItem["创建者"] = appraiseUser.ID;// + ";#" + appraiseUser.Name;//当前用户
                                listItem.Update();
                            }
                            spWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
        }
        //返回新建项目的Id
        private   void WritePerform( float intRadio, PickerEntity picker)
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
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
                                listItem[GetTitleDispName] = ID;// + ";#" + strPerformance;
                            }

                            else
                            {
                                int id = int.Parse(ViewState["id"].ToString());
                                listItem = spList.Items.GetItemById(id);
                            }
                            listItem[GetAuthorDispName] =  GetUserValue(spWeb, picker);//用户填写的
                            listItem [GetRadioDispName] = intRadio;//系数
                            listItem["动作"] = webObj.CurrentAction;
                            listItem["创建者"] = appraiseUser.ID;// + ";#" + appraiseUser.Name;//当前用户
                            listItem.Update();
                             spWeb.AllowUnsafeUpdates = false;
                         }
                    }
                }
            });
         }
        //获取控件中的值
        private SPFieldUserValue GetUserValue(SPWeb web, PickerEntity picker)
        {
            SPUser user = web.EnsureUser(picker.Key);
            SPFieldUserValue fieldvalue = new SPFieldUserValue(web, user.ID, user.LoginName);
            return fieldvalue;
        }
        /// <summary>
        /// 删除业绩
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePerformRecord(int id)
        {
            using (SPSite spSite = new SPSite(SPContext.Current.Site.ID )) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);
                    spList.Items.DeleteItemById(id);
                    return true;
                }
            }

        }
        //获取与论文相关的业绩，
        public DataTable  GetPerformRecord(string strAction, string strPerformanceID)
        {
            string siteUrl = SPContext.Current.Site.Url;
            //List<string> authors = new List<string>();
            IDictionary<int, string> authors = new Dictionary<int, string>();
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);
                    string lookupInternalName = GetSubTitleInterlname.Trim();
                    if (spList != null)
                    {
                        SPQuery qry = new SPQuery();
                        string strQuery = @"<Where><And><Eq><FieldRef Name='Action' /><Value Type='Choice'>" + strAction + "</Value></Eq><Eq><FieldRef Name='"+lookupInternalName +"' LookupId='True' /><Value Type='Lookup'>" + strPerformanceID + "</Value></Eq></And></Where>";
                        qry.Query = strQuery;
                        SPListItemCollection listItems = spList.GetItems(qry);
                        rblAuthors.Items.Clear();
                        double tRatio = 0;
                        string authorNameTitle = spList.Fields.GetFieldByInternalName("AuthorName").Title;
                        foreach (SPListItem listItem in listItems )
                        {
                            SPFieldUserValue author = new SPFieldUserValue(spWeb, listItem[authorNameTitle].ToString());
                            //double  ratio = listItem[GetRadioDispName]==null?0: (double )listItem[GetRadioDispName] ;
                            //tRatio = tRatio + ratio;
                            //rblAuthors.Items.Add(new ListItem(author.User.Name +"  "+ratio , author.User.ID + ";" + listItem.ID+";"+ratio  ));
                            authors.Add(listItem.ID, author.User .LoginName);
                        }
                        ViewState["TotalRatio"] = tRatio;
                        ViewState["Authors"] = authors;
                        DataTable dt = listItems.GetDataTable();
                        if (dt == null)
                            dt = spList.GetItems().GetDataTable().Clone() ;

                        DataTable dt1 = dt.Copy();
                        if (dt1.Columns.Contains("Radio"))
                        {
                            dt1.Columns["Radio"].ColumnName = "Ratio"; 
                        }
                        int id = -1;
                        for (int i = dt1.Rows.Count; i < webObj.TotolRows; i++)
                        {
                            DataRow dr = dt1.NewRow();
                            dr["ID"] = id;
                            dt1.Rows.Add(dr);
                            id = id - 1;
                        }
                        if (dt.Rows.Count == 0)
                        {
                            SPUser use = SPContext.Current.Web.CurrentUser;
                            dt1.Rows[0]["AuthorName"] = use.ID;
                            dt1.Rows[0]["Ratio"] = TotalRatio;
                            authors.Add((int)dt1.Rows[0]["ID"],use.LoginName );
                        }

                        ViewState["gvSource"] = dt1;
                        return dt1 ;

                    }
                }
            }

            return null;
        }
        //获取业绩子表的关联字段内部名，显示名称同主表标题，查阅项
        private string GetSubTitleInterlname
        {
            get
            {
                if (ViewState["subFieldInterlName"] != null)
                    return ViewState["subFieldInterlName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.ResultList);
                    if (list != null)
                    {
                        string displayName = GetTitleDispName;//获取主表的标题显示名称
                        SPField field = list.Fields.GetField(displayName);
                        ViewState["subFieldInterlName"] = field.InternalName;
                        return field.InternalName;
                    }
                    else
                        return "";
                }
            }
        }
        #endregion
        #region 暂时未用的方法，
        private void SaveOld()
        {
            //		acounts	"i:0#.w|ccc\\wangboran,i:0#.w|ccc\\1601728,i:0#.w|ccc\\1601733,i:0#.w|ccc\\1601731"	string
            if (tbName.CommaSeparatedAccounts == "" || tbRatio.Text == "")
            {
                lblMsg.Text = lblName.Text + "或" + lblRatio.Text + "不能为空！";
                return;
            }
            if (ViewState["id"] == null)
            {
                if (!CheckAddUser(decimal .Parse(tbRatio.Text)))
                    return;
                //ArrayList newAuthors = new ArrayList();
                foreach (PickerEntity picker in tbName.ResolvedEntities)
                {
                    //ListItem item = rblAuthors.Items.FindByText (picker.DisplayText);//文本中显示姓名和业绩
                    bool isNew = true;//userb 测试用户
                    foreach (ListItem item in rblAuthors.Items)
                    {

                        if (item.Text.StartsWith(picker.DisplayText))
                        {
                            isNew = false;
                            break;
                        }
                        else
                        {
                            string account = picker.Key.Substring(picker.Key.LastIndexOf("\\") + 1);
                            if (item.Text.StartsWith(account))
                            {
                                isNew = false;
                                break;
                            }
                        }
                    }
                    if (isNew)
                    {
                        WritePerform(tbRatio.Text == "" ? 0 : float.Parse(tbRatio.Text.Trim()), picker);
                        //重新填充
                        InitAuthors();
                    }
                    else
                        lblMsg.Text = webObj.ShowMsgRepeat;
                }
            }
            else//编辑
            {
                decimal tRatio = (decimal)ViewState["TotalRatio"];//分配业绩和
                decimal bRation = (decimal)ViewState["beforeRatio"];
                if (tRatio - bRation + decimal .Parse(tbRatio.Text) > TotalRatio)
                {
                    lblMsg.Text = webObj.ShowMsg.Replace("N", (TotalRatio - tRatio + bRation).ToString());
                    setFocus();
                }
                else
                {
                    lblMsg.Text = "";
                    WritePerform(tbRatio.Text == "" ? 0 : float.Parse(tbRatio.Text.Trim()), (PickerEntity)tbName.ResolvedEntities[0]);
                    //重新填充
                    InitAuthors();

                }
            }
        }
        private void NewClick()
        {
            bool allowNew = CheckAddUser();
            if (allowNew)
            {
                if (rblAuthors.SelectedItem != null)
                    rblAuthors.SelectedItem.Selected = false;
                NewClaim();
            }
        }
        /// <summary>
        //是否可以 创建新的业绩，根据用户个数
        //总当量是不是已经分完
        /// </summary>
        /// <param name="cRation">新建业绩的值</param>
        /// <returns></returns>
        private bool CheckAddUser(decimal  cRation = 0)
        {

            if (ViewState["TotalRatio"] == null && cRation <= TotalRatio)
                return true;
            decimal tRatio = (decimal )ViewState["TotalRatio"];
            if (tRatio + cRation > TotalRatio)
            {
                lblMsg.Text = webObj.ShowMsg.Replace("N", (TotalRatio - tRatio).ToString());
                setFocus();
                return false;
            }
            else
            {
                lblMsg.Text = "";
                return true;
            }
        }

        //准备创建新业绩分配
        private void NewClaim()
        {
            ViewState["id"] = null;
            if (ViewState["TotalRatio"] == null)
                tbRatio.Text = TotalRatio.ToString();
            else
                tbRatio.Text = (TotalRatio - (decimal)ViewState["TotalRatio"]).ToString();
            tbName.CommaSeparatedAccounts = "";

        }
        private void InitAuthors()
        {
            string ID = Page.Request.QueryString["ID"];//论文ID
            //读取已有的业绩分配
            //SPListItemCollection items = GetPerformRecord(webObj.CurrentAction, ID);//获取当前论文的业绩
            NewClaim();
        }
        /// <summary>
        /// 论文的作者，Authors用分号分开的字符串；院内作者，AuthorName用户或用户组，允许多重选择
        /// </summary>
        /// <returns></returns>
        private string[] GetPaperAuthors()
        {
            string ID = Page.Request.QueryString["ID"];
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(webObj.FromList);
            if (list != null)
            {
                SPListItem itme = list.GetItemById(int.Parse(ID));
                if (itme != null && itme[GetAuthorsDispName] != null)
                {
                    string authors = itme[GetAuthorsDispName].ToString().Trim().TrimEnd(';');
                    string[] users = Regex.Split(authors, ";");
                    return users;
                }
            }
            return new string[] { };
        }

        /// <summary>
        /// 获取当前主表的系数和，更改为按百分比，总业绩为1
        /// </summary>
        /// <returns></returns>
        private double GetRatio()
        {
            string ID = Page.Request.QueryString["ID"];
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(webObj.FromList);
            if (list != null)
            {
                SPListItem itme = list.GetItemById(int.Parse(ID));
                if (itme != null)
                {
                    return itme[webObj.FromRatioField] == null ? 0 : (double)itme[webObj.FromRatioField];
                }

            }
            return 0;
        }
        #endregion
    }
}
