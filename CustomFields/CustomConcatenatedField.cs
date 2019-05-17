using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Specialized;

namespace CustomFields
{
    //定义字段
    public class CustomConcatenatedField : SPFieldLookup
    {
        const string CustFieldLookupList = "custFieldLookuplist";
        //创建字段保存创建配置属性： 
        const string CustFieldLookupField = "custFieldLookupfield";
        const string CustFieldWidth = "custWidth";

        public CustomConcatenatedField(SPFieldCollection fields, string fieldName) : base(fields, fieldName)
        {
            _spLookupList =""+ base.GetCustomProperty(CustFieldLookupList) ;
            _spLookupField = "" + base.GetCustomProperty(CustFieldLookupField);
            _spLookupWidth = "" + base.GetCustomProperty(CustFieldWidth);
        }

        public CustomConcatenatedField(SPFieldCollection fields, string typeName, string displayName) : base(fields, typeName, displayName)
        {
            _spLookupList =""+ base.GetCustomProperty(CustFieldLookupList) ;
            _spLookupField = "" + base.GetCustomProperty(CustFieldLookupField);
            _spLookupWidth = "" + base.GetCustomProperty(CustFieldWidth);
        }
        public override bool AllowMultipleValues
        {
            get
            {
                return false;
            }

            set
            {
                base.AllowMultipleValues = value;
            }
        }

        public override object GetFieldValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return new SPFieldLookupValue(value);
        }

        private string _spLookupList;
        private string _spLookupField;
        private string _spLookupWidth;
        public string SPLookupWidth
        {
            get
            {
                return _spLookupWidth ;
            }
            set
            {
                _spLookupWidth = value;
                this.SetCustomPropertytoCache(CustFieldWidth, value);
            }
        }
        public string SPLookupList
        {
            get
            {
                return _spLookupList;
            }
            set
            {
                _spLookupList = value;
                this.SetCustomPropertytoCache(CustFieldLookupList, value);
            }
        }
        public string SPLookupField
        {
            get
            {
                return _spLookupField;
            }
            set
            {
                _spLookupField = value;
                this.SetCustomPropertytoCache(CustFieldLookupField, value);
            }
        }
        private static readonly Dictionary<string, StringDictionary>
            CustomPropertiesCache = new Dictionary<string, StringDictionary>();
        private string ContextKey
        {
            get
            {
                return this.ParentList.ID.ToString() + "_" + System.Web.HttpContext.Current.GetHashCode();
            }
        }
        protected void SetCustomPropertytoCache(string key, string value)
        {
            StringDictionary plist = null;
            if (CustomPropertiesCache.ContainsKey(ContextKey))
            {
                plist = CustomPropertiesCache[ContextKey];
            }
            else
            {
                plist = new StringDictionary();
                CustomPropertiesCache.Add(ContextKey, plist);
            }
            if (plist.ContainsKey(key))
            {
                plist[key] = value;
            }
            else
            {
                plist.Add(key, value);
            }
        }
        protected string GetCustomPropertyFromCache(string key)
        {
            if (CustomPropertiesCache.ContainsKey(ContextKey))
            {
                StringDictionary plist = CustomPropertiesCache[ContextKey];
                if (plist.ContainsKey(key))
                    return plist[key];
                else
                    return "";
            }
            else
            {
                return "";
            }
        }
        public override void OnAdded(SPAddFieldOptions op)
        {
            base.OnAdded(op);
            Update();
        }
        public override void Update()
        {

            base.SetCustomProperty(CustFieldLookupList, this.GetCustomPropertyFromCache(CustFieldLookupList ));
            base.SetCustomProperty(CustFieldLookupField, this.GetCustomPropertyFromCache(CustFieldLookupField ));
            base.SetCustomProperty(CustFieldWidth, this.GetCustomPropertyFromCache(CustFieldWidth));
            base.Update();
        }

        public override BaseFieldControl FieldRenderingControl
        {
            get
            {
                BaseFieldControl fieldControl = new CustomConcatenatedFieldControl();       //在下面的内容中定义
                fieldControl.FieldName = this.InternalName;
                return fieldControl;
            }
        }
        /// <summary>
        /// 提交表单时候的验证数据类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetValidatedString(object value)
        {
            string strValue = "" + value;
            if (Required && strValue == "")
            {
                throw new SPFieldValidationException("不能为空");
            }

            return base.GetValidatedString(value);
        }
    }

}
