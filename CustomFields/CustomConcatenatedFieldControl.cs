using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace CustomFields
{
    //展示字段
    public class CustomConcatenatedFieldControl : BaseFieldControl
    {
        #region 控件定义及重写方法
        //选项
        protected DropDownList DropDownList1;
        protected DropDownList ddlDes;
        protected DropDownList ddlNew;
        protected Label lblDisplayText;
        protected LookupField fldAction;
        protected HtmlTable tblLevel;
        protected HtmlGenericControl spanDesc;
        protected HtmlGenericControl spanTxtDesc;
        protected TextBox txtCustAction;

        protected override void CreateChildControls()
        {
            if (Field == null) return;
            base.CreateChildControls();
            SPFieldLookupValue  fLookValue = (SPFieldLookupValue )this.ItemFieldValue;
            fldAction = (LookupField)TemplateContainer.FindControl("fldAction");

            if (this.ControlMode == SPControlMode.Display)
            {
                lblDisplayText = (Label)TemplateContainer.FindControl("lblDisplayText");

                if (lblDisplayText != null)
                {
                    if (fLookValue != null)
                    {
                        lblDisplayText.Text =fLookValue.LookupValue;// showText.TrimEnd('：');
                    }
                }
            }
            else
            {
                this.DropDownList1 = (DropDownList)TemplateContainer.FindControl("DropDownList1");
                this.ddlDes = (DropDownList)TemplateContainer.FindControl("ddlDes");
                this.spanDesc  = (HtmlGenericControl )TemplateContainer.FindControl("spanDesc");
                this.spanTxtDesc  = (HtmlGenericControl )TemplateContainer.FindControl("spanTxtDesc");
                this.tblLevel = (HtmlTable)TemplateContainer.FindControl("tblLevel");
                
                DropDownList1.AutoPostBack = true;
                DropDownList1.SelectedIndexChanged += Ddl_SelectedIndexChanged;

                //CustomConcatenatedField field = (CustomConcatenatedField)base.Field;
                //新建或编辑值为空时，进行初始化填充
                if (fLookValue == null)
                {
                    if (ViewState["lastID"] != null)
                    {
                        string lastID = ViewState["lastID"].ToString();

                        InitControls(int.Parse(lastID));
                    }
                    else
                        InitControls();
                }
                else
                {
                    int id;//编辑过程中，更改选项
                    if (ViewState["lastID"] != null)
                        id = int.Parse(ViewState["lastID"].ToString());
                    else
                        id = fLookValue.LookupId;

                    //填充值
                    InitControls(id);
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">新的操作名称</param>
        /// <param name="typeFldName">类别字段名称</param>
        /// <param name="typeID">类别值</param>
        private int AddOperate(string action, int typeID)
        {
            CustomConcatenatedField f = (CustomConcatenatedField)base.Field;
            string lstID = f.LookupList;
            SPSite s = SPControl.GetContextSite(Context);
            int itemID=0;
            SPWeb lookupWeb = SPControl.GetContextWeb(Context);
            int userID = lookupWeb.CurrentUser.ID;
            SPField typeFld = GetCascadedField;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(lookupWeb.Site.ID))
                {
                    using (SPWeb web = site.AllWebs[lookupWeb.ID])
                    {
                        SPList lookupList = web.Lists[new Guid(lstID)];
                        SPListItem addItem = lookupList.Items.Add();
                        addItem["Title"] = action;
                        addItem[typeFld.Title] = typeID;
                        addItem[lookupList.Fields.GetFieldByInternalName("Flag").Title] = "11";
                        addItem["Author"] = userID;
                        web.AllowUnsafeUpdates = true;
                        addItem.Update();
                        web.AllowUnsafeUpdates = false;
                        itemID  = addItem.ID;
                    }
                }
            });
            return itemID;
        }
        /// <summary>
        /// 最后一级的值为要保存的值
        /// </summary>
        public override object Value
        {
            get
            {
                int level = tblLevel.Rows.Count;
                string ddlID = "DropDownList" + level;
                this.ddlNew = (DropDownList)TemplateContainer.FindControl(ddlID);
                SPFieldLookupValue f = null;
                this.txtCustAction = (TextBox)TemplateContainer.FindControl("txtNewAction");//动态加的
                //添加的自定义操作是否存在
                if (txtCustAction != null && txtCustAction.Text.Length > 0)
                {
                    ListItem fldItem = ddlNew.Items.FindByText(txtCustAction.Text);
                    if (fldItem == null)
                    {
                        int id = AddOperate(txtCustAction.Text, int.Parse(DropDownList1.SelectedItem.Value));
                        fldItem = new System.Web.UI.WebControls.ListItem(txtCustAction.Text, id.ToString());
                        ddlNew.Items.Add(fldItem);
                        ddlNew.Items[ddlNew.Items.Count - 1].Selected = true;
                        f = new SPFieldLookupValue(id, txtCustAction.Text);
                    }
                }
                if (ddlNew.Items.Count == 0)
                    return null;
                if (f == null)
                    f = new SPFieldLookupValue(Convert.ToInt32(ddlNew.SelectedValue), ddlNew.SelectedItem.Text);

                return f;
            }
            set
            {
                this.EnsureChildControls();
                base.Value = value as SPFieldLookupValue;
            }
        }
        public override void UpdateFieldValueInItem()
        {
            base.ItemFieldValue = Value;
        }

        #endregion
        #region 初始化

        private void InitControls(int id = 0, int level = 2)
        {
            bool multiLevel = MultiLevel;
            CustomConcatenatedField f = (CustomConcatenatedField)base.Field;
            SPFieldLookup fldParent = GetCascadedField;
            string parentField = multiLevel ? fldParent.InternalName : "";
            List<ListItem> itemDesc = new List<ListItem>();
            ListItem[] cateItems = GetListItems(fldParent.LookupList, fldParent.LookupField, parentField, 0, ref itemDesc);

            DropDownList1.Items.Clear();
            DropDownList1.Items.AddRange(cateItems);//顶级先填充
            ddlDes.Items.Clear();
            if (itemDesc.Count > 0)
            {

                ddlDes.Items.AddRange(itemDesc.ToArray());
            }


            if (f.SPLookupWidth != "")
                DropDownList1.Width = int.Parse(f.SPLookupWidth);

            //顶级不变，从第二开始
            List<int> selectdValues = new List<int>();
            if (id > 0)//以前有值 
            {
                selectdValues.Add(id);
                GetSeletedParentIDs(id, multiLevel, ref selectdValues);
                selectdValues.Reverse();//自下而上改为自上而下
                if (selectdValues[0] == -1)//处理操作被删除的情况
                {
                    selectdValues[0] = int.Parse(DropDownList1.Items[0].Value);
                }

                DropDownList1.SelectedValue = selectdValues[0].ToString();


            }
            else//以前无值
            {
                id = int.Parse(DropDownList1.SelectedValue);//查找以当前ID为父ID的项
                selectdValues.Add(id);
                GetAllIDs(id, multiLevel, ref selectdValues);
            }
            if (ddlDes.Items.Count > 0)
                spanDesc.InnerText = ddlDes.Items[DropDownList1.SelectedIndex].Text;

            FillChildLevel(selectdValues, level);
        }
        #endregion
        #region 自定义 方法
        /// <summary>
        /// 填充自上向下的级别数
        /// </summary>
        /// <returns></returns>
        private void FillChildLevel(List<int> selectedValues, int level)
        {
            CustomConcatenatedField f = (CustomConcatenatedField)base.Field;
            SPFieldLookup fld = GetCascadedField;
            ListItem[] items;
            bool multiLevel = MultiLevel;
            int tEnd = selectedValues.Count - 1;//只有两级时，查阅项字段在另一个列表中，否则遍历会进入死
            bool allowAdd = false;
            if (tEnd == 0)//类别下操作为空，会出错2018-11-16
            {
                tEnd = 1;
                allowAdd = true;
            }
            //循环
            string ddlID;
            if (multiLevel)
                tEnd = selectedValues.Count;
            int parentID;
            HtmlTableRow row;
            HtmlTableCell cell;
            for (int i = 0; i < tEnd; i++)
            {
                parentID = selectedValues[i];
                List<ListItem> lstDesc = new List<System.Web.UI.WebControls.ListItem>();
                items = GetListItems(f.LookupList, f.LookupField, fld.InternalName, parentID, ref lstDesc);
                ddlID = "DropDownList" + level;
                this.ddlNew = (DropDownList)TemplateContainer.FindControl(ddlID);
                if (items.Length > 0||allowAdd )//还有下一级
                {
                    if (ddlNew == null)
                    {
                        ddlNew = new DropDownList();
                        ddlNew.ID = ddlID;
                        if (f.SPLookupWidth != "")
                            ddlNew.Width = int.Parse(f.SPLookupWidth);
                         row = new HtmlTableRow();
                         cell = new HtmlTableCell();
                        cell.Controls.Add(ddlNew);
                        row.Cells.Add(cell);
                        tblLevel.Rows.Add(row);

                        cell = new HtmlTableCell();
                        row.Cells.Add(cell);
                        tblLevel.Rows.Add(row);
                    }
                    else
                    {
                        if (f.SPLookupWidth != "")
                            ddlNew.Width = int.Parse(f.SPLookupWidth);
                    }
                    if (multiLevel)
                    {
                        ddlNew.AutoPostBack = true;
                        ddlNew.SelectedIndexChanged += Ddl_SelectedIndexChanged;
                    }
                    ddlNew.Items.Clear();
                    ddlNew.Items.AddRange(items);
                    if ((i + 1) < selectedValues.Count)
                        ddlNew.SelectedValue = selectedValues[i + 1].ToString();
                    level = level + 1;
                }
            }
            //保留最后一级的选项
            if (multiLevel)
            {
                ViewState["lastID"] = selectedValues[tEnd - 1].ToString();//保留最后一级当前所选ID，临时的
                //txtCustAction.Visible = false;
                spanTxtDesc.Visible = false;
            }
            else//二级的，添加自定义操作
            {
                ViewState["lastID"] = null;
                this.txtCustAction = (TextBox)TemplateContainer.FindControl("txtNewAction");//动态加的
                if (txtCustAction == null)
                {
                    TextBox txtNewAction = new TextBox();
                    txtNewAction.ID = "txtNewAction";
                   
                    if (f.SPLookupWidth != "")
                        txtNewAction.Width = int.Parse(f.SPLookupWidth) - 10;
                    //txtCustAction.Visible = true;
                    
                    tblLevel.Rows[level - 2].Cells[1].Controls.Add(txtNewAction);
                }
                spanTxtDesc.Visible = true;
            }
            //原来的是三级，更改选项后变成了两级，级别减少了
            for (int j = tblLevel.Rows.Count; j >= level; j--)
            {
                tblLevel.Rows.RemoveAt(j - 1);
            }
        }

        /// <summary>
        /// 自上而下获取ID
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="ids"></param>
        private void GetAllIDs(int parentID, bool multiLevel, ref List<int> ids)
        {
            CustomConcatenatedField f = (CustomConcatenatedField)base.Field;
            SPFieldLookup fld = GetCascadedField;
            int id = GetListItemsOfFirstID(f.LookupList, f.LookupField, fld.InternalName , parentID);
            if (id > 0)
            {
                ids.Add(id);
                if (multiLevel)
                    GetAllIDs(id, multiLevel, ref ids);
            }
        }
        //自下而上获取选项值，已有值（编辑时进行填充）
        private void GetSeletedParentIDs(int id, bool multiLevel, ref List<int> parentIDs)
        {
            SPFieldLookup fld = GetCascadedField;//查阅项列表中的父类（查阅项类型）字段

            id = GetParentID(id, fld.Title);
            if (id != 0)
                parentIDs.Add(id);
            if (multiLevel && id>0)
                GetSeletedParentIDs(id, multiLevel, ref parentIDs);
        }

        //返回查阅项表的类别ID(ParentID)，通过ParentID进行关联
        private int GetParentID(int id, string pDisplayField)
        {
            SPList lst = GetLookupList;
            try
            {
                SPListItem itm = lst.GetItemById(id);//值不存在会报错

                if (itm[pDisplayField] != null)
                {
                    SPFieldLookupValue fValue = new SPFieldLookupValue(itm[pDisplayField].ToString());
                    return fValue.LookupId;
                }
                else
                    return 0;
            }
            catch
            {
                return -1;
            }

        }
        /// <summary>
        /// 返回当前父级下的第一项的ID
        /// </summary>
        /// <param name="lstID"></param>
        /// <param name="fldInterlName"></param>
        /// <param name="fldCascaded"></param>
        /// <param name="cateID"></param>
        /// <returns></returns>
        private int GetListItemsOfFirstID(string lstID, string fldInterlName, string fldCascaded, int cateID)
        {

            SPSite s = SPControl.GetContextSite(Context);
            SPWeb lookupWeb = SPControl.GetContextWeb(Context);
            SPList lookupList = lookupWeb.Lists[new Guid(lstID)];
            SPQuery qry = new SPQuery();
            if (fldCascaded == "")//没有层次级别的分类，直接获取表中的所有数据
                qry.Query = @"";
            else //通过父类ID进行层次级别的标识
            {
                if (cateID > 0)
                    qry.Query = @"<Where><Eq><FieldRef Name='" + fldCascaded + "' LookupId='True' /><Value Type='Lookup'>" + cateID + "</Value></Eq></Where>";
                else
                    qry.Query = @"<Where><IsNull><FieldRef Name='" + fldCascaded + "' /></IsNull></Where>";
            }

            //查阅项列表
            SPListItemCollection items = lookupList.GetItems(qry);
            int id = 0;
            if (items.Count > 0)
                id = items[0].ID;
            return id;
        } 
        /// <summary>
        /// 返回级联相关的数据
        /// </summary>
        /// <param name="lstID">查阅项列表iD</param>
        /// <param name="fldInterlName">查阅项显示字段</param>
        /// <param name="fldCascaded">父类字段（查阅项字段）</param>
        /// <param name="cateID"></param>
        /// <returns></returns>
        private  ListItem[] GetListItems(string  lstID, string fldInterlName, string fldCascaded, int cateID,ref List<ListItem> itemDesc  )
        {
           
            SPSite s = SPControl.GetContextSite(Context);
            SPWeb lookupWeb = SPControl.GetContextWeb(Context);
            SPList lookupList = lookupWeb.Lists[new Guid ( lstID)];
            SPQuery qry = new SPQuery();
            if (fldCascaded == "")//二级的顶级
            {
                //填充类别
                if (lookupList.Fields.ContainsFieldWithStaticName("SN"))
                    qry.Query = @"<OrderBy><FieldRef Name='SN' Ascending='true' /></OrderBy>";
                else
                    qry.Query = @"";
            }
            else//多级的顶级及下一级的填充
            {
                if (lookupList.Fields.ContainsFieldWithStaticName("Flag"))
                {
                    int userID = SPContext.Current.Web.CurrentUser.ID;
                    if (cateID > 0)
                        qry.Query = @"<Where><And><Eq><FieldRef Name='" + fldCascaded + "' LookupId='True' /><Value Type='Lookup'>" + cateID + "</Value></Eq><Or><Eq><FieldRef Name='Flag' /><Value Type='Number'>0</Value></Eq><And><Eq><FieldRef Name='Flag' /><Value Type='Number'>11</Value></Eq><Eq><FieldRef Name = 'Author' LookupId = 'TRUE'></FieldRef><Value Type = 'User'>" + userID + "</Value></Eq></And></Or></And></Where>";
                    else
                        qry.Query = @"<Where><And><IsNull><FieldRef Name='" + fldCascaded + "' /></IsNull><Or><Eq><FieldRef Name='Flag' /><Value Type='Number'>0</Value></Eq><And><Eq><FieldRef Name='Flag' /><Value Type='Number'>11</Value></Eq><Eq><FieldRef Name = 'Author' LookupId = 'TRUE'></FieldRef><Value Type = 'User'>" + userID + "</Value></Eq></And></Or></And></Where>";

                }
                else
                {
                    if (cateID > 0)
                        qry.Query = @"<Where><Eq><FieldRef Name='" + fldCascaded + "' LookupId='True' /><Value Type='Lookup'>" + cateID + "</Value></Eq></Where>";
                    else
                        qry.Query = @"<Where><IsNull><FieldRef Name='" + fldCascaded + "' /></IsNull></Where>";
                }
            }
 
            //查阅项列表
            SPListItemCollection items = lookupList.GetItems(qry);
            List<ListItem> desItems = new List<ListItem>();
            
            SPField lookupField = lookupList.Fields.GetFieldByInternalName(fldInterlName=="ID"?"Title":fldInterlName );

            Guid FieldID = lookupField.Id;
            
            foreach (SPListItem i in items)
            {

                ListItem _s = new System.Web.UI.WebControls.ListItem(
                     i.Fields[FieldID].GetFieldValueAsText(i[FieldID]), i.ID.ToString());
                desItems.Add(_s);
                string fldDescName = "Desc";
                if (fldCascaded == "" && lookupList.Fields.ContainsFieldWithStaticName(fldDescName))
                {
                    SPField fld = lookupList.Fields.GetFieldByInternalName(fldDescName);
                    ListItem _sDes = new ListItem(i.Fields[fld.Id].GetFieldValueAsText(i[fld.Id ]),i.ID.ToString () );
                    itemDesc.Add(_sDes);
                }
            }
            return desItems.ToArray(); ;
        }
        #endregion
        #region 自定义 属性
        private SPFieldLookup fLookup = null;
        /// <summary>
        /// 获取查阅项列表中（操作列表中的类别，任务列表中的，父 ID）级别字段，也就是带有查阅项的字段
        /// </summary>
        /// <param name="lookupList"></param>
        /// <returns>parentID,如果存在，则会有多级</returns>
        private SPFieldLookup GetCascadedField
        {
            get
            {
                if (fLookup == null)
                {
                    SPList lookupList = GetLookupList;// lookupWeb.Lists[new Guid(f.SPLookupList)];
                    string fName = "ParentID";//查阅项在同一列表中
                    if (lookupList.Fields.ContainsFieldWithStaticName(fName))
                    {
                        SPField fld = lookupList.Fields.GetFieldByInternalName(fName);
                        if (fld.Type == SPFieldType.Lookup)
                        {
                            fLookup = fld as SPFieldLookup;
                        }
                    }
                    else
                    {
                        foreach (SPField fld in lookupList.Fields)
                        {
                            if (fld.Type == SPFieldType.Lookup)
                            {

                                fLookup = fld as SPFieldLookup;//查阅项不在同一个列表中
                                if (new Guid(fLookup.LookupList) != lookupList.ID)
                                    break;

                            }

                        }
                    }
                }
                return fLookup;
            }
        }
        ////获取级联数据
        SPList _lookupList;
        private SPList GetLookupList
        {
            get
            {
                if (_lookupList == null)
                {
                    CustomConcatenatedField f = (CustomConcatenatedField)base.Field;
                    SPSite s = SPControl.GetContextSite(Context);
                    SPWeb lookupWeb = SPControl.GetContextWeb(Context);
                    _lookupList = lookupWeb.Lists[new Guid(f.SPLookupList)];
                }
                return _lookupList;
            }
            set
            {

            }
        }
        private bool _parentLeve;
        /// <summary>
        /// 是否是多个级别,查询项的列表包含父ID（查询项类型）-是多级，如任务列表；
        /// 否则为二级（查询项的列表含查询项字段，例如：“操作”列表中“操作类别”字段 ，而查阅自另一个列表）
        /// </summary>
        private bool MultiLevel
        {
            get
            {
                if (ViewState["_parentLeve"] == null)//查阅项字段的列表就是当前列表，为多级
                {
                    SPList lookupList = GetLookupList;
                    SPFieldLookup lookupField = GetCascadedField;
                    _parentLeve = new Guid(lookupField.LookupList) == lookupList.ID ? true : false;
                    ViewState["_parentLeve"] = _parentLeve;
                }
                else
                    _parentLeve = (bool)ViewState["_parentLeve"];
                return _parentLeve;

            }
        }
        #endregion
        #region 自定义事件
        /// <summary>
        /// 选项改变的时候，填充当前级下面的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ddl_SelectedIndexChanged(object sender, EventArgs e)
        {

            DropDownList ddl = (DropDownList)sender;
            if (ddl.ID.EndsWith("1") && ddlDes.Items.Count > 0)//第一级选项更改的事件
                spanDesc.InnerText = ddlDes.Items[DropDownList1.SelectedIndex].Text;
            int level = int.Parse(ddl.ID.Replace("DropDownList", "")) + 1;
            int id = int.Parse(ddl.SelectedValue);//查找以当前ID为父ID的项
            List<int> selectdValues = new List<int>();
            selectdValues.Add(id);

            GetAllIDs(id, MultiLevel, ref selectdValues);
            FillChildLevel(selectdValues, level);
        }
        #endregion
        #region 重写属性
        //重写默认模板
        protected override string DefaultTemplateName
         {
             get
             {
                 if (this.ControlMode == SPControlMode.Display)
                 {
                     return this.DisplayTemplateName;
                 }
                 else
                 {
                    return "DefaultCustomFieldControl";  
                 }
             }
         }
        public override string DisplayTemplateName
        {
            get
            {
                return "DisplayCustomFieldControl";///DisplayCustomFieldControl
            }
            set
            {
                base.DisplayTemplateName = value;
            }
        }
        #endregion
    }
}
