using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CustomFields
{
    public class CustomConcatenatedFieldEditor : UserControl, IFieldEditor
    {
        //定义参数的位置
        bool IFieldEditor.DisplayAsNewSection
        {
            get { return false; }
        }
        public void InitializeWithField(SPField field) { }
        #region 自定义 事件 
        //定义编辑字段的两个控件
        protected DropDownList ddlListName = null;
        protected DropDownList ddlFieldName = null;
        protected TextBox txtWidth=null;
        //编辑字段配置信息绑定到控件上
        void IFieldEditor.InitializeWithField(SPField field)
        {
            if (!Page.IsPostBack)
            {
                this.EnsureChildControls();
                BindSPListFieldData(ddlListName);
                if (ddlListName.Items.Count > 0 && ddlFieldName.Items.Count == 0)
                    FillListFields(ddlListName.Items[0].Value);
            }

            if (!Page.IsPostBack && field != null)
            {
                CustomConcatenatedField fields = (CustomConcatenatedField)field;
                this.ddlListName.SelectedValue = fields.SPLookupList;
                ddlFieldName.SelectedValue = fields.SPLookupField;
                txtWidth.Text = fields.SPLookupWidth;
            }
            ddlListName.AutoPostBack = true;
            ddlListName.SelectedIndexChanged += DdlListName_SelectedIndexChanged;
        }
        //填充指定列表下的字段
        private void FillListFields(string listGuid)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPContext.Current.Web.Site.ID))
                {
                    using (SPWeb web = site.AllWebs[SPContext.Current.Web.ID])
                    {
                        Guid listid = new Guid(listGuid);
                        SPList splist = web.Lists[listid];
                        SPFieldCollection splistfield = splist.Fields;
                        ddlFieldName.Items.Clear();
                        ListItem litem;
                        foreach (SPField spfsitem in splistfield)
                        {
                            if (spfsitem.Reorderable)
                            {
                                if (spfsitem.Type == SPFieldType.Text && !spfsitem.ReadOnlyField)//
                                {

                                    string _text = spfsitem.Title;
                                    string _value = spfsitem.InternalName;
                                    litem = new System.Web.UI.WebControls.ListItem(_text, _value);
                                    ddlFieldName.Items.Add(litem);

                                }
                            }
                        }
                    }
                }
            });
        }
        /// <summary>
        /// 绑定选定列表中的所有字段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdlListName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillListFields(ddlListName.SelectedValue);
        }

        //绑定当前网站下的所有列表到下拉控件;2018-9-29提升权限
        void BindSPListFieldData(DropDownList drlist)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {   
            using (SPSite site = new SPSite(SPContext.Current.Web.Site.ID))
            {
                using (SPWeb web = site.AllWebs[SPContext.Current.Web.ID])
                {
                    drlist.Items.Clear();
                    ListItem litem;

                    foreach (SPList lst in web.Lists)
                    {
                        if (lst.BaseType == SPBaseType.GenericList)
                        {
                            litem = new ListItem(lst.Title, lst.ID.ToString());
                            drlist.Items.Add(litem);
                        }

                    }

                }
            }
             });
        }
        #endregion
        void IFieldEditor.OnSaveChange(SPField field, bool isNewField)
        {
            this.EnsureChildControls();
            if (field != null)
            {
                CustomConcatenatedField _f = field as CustomConcatenatedField;

                SPSite _site = SPControl.GetContextSite(this.Context);
                SPWeb _web = _site.OpenWeb();
                _f.LookupWebId = _web.ID;
                _f.LookupList = ddlListName.SelectedValue;
                _f.LookupField = ddlFieldName.SelectedValue;
                _f.SPLookupList = ddlListName.SelectedValue;
                _f.SPLookupField = ddlFieldName.SelectedValue;
                _f.SPLookupWidth = txtWidth.Text;
                
            }
        }
    }
}
