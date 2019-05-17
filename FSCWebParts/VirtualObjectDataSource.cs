using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace FSCWebParts
{
    /// <summary>
    /// 分页增强的ObjectDataSource.
    /// </summary>
    public class VirtualObjectDataSource : ObjectDataSource
    {
        private GridViewPagingAdapter m_pagingAdapter;
        public VirtualObjectDataSource(DataTable dataSource, int virtualItemCount)
        {
            // 设置数据实体类型，可不设（它自己会辨认的）
            this.DataObjectTypeName = string.Empty;
            // 当然要分页的，要不然就没必要这么做了.
            this.EnablePaging = true;
            // 页数是虚拟的，自然ViewState就没有意义了。
            this.EnableViewState = false;

            /// 设置分页需要的属性和方法.
            this.TypeName = "GridViewPagingAdapter";
            this.SelectMethod = "GetData";
            this.SelectCountMethod = "VirtualItemCount";
            this.StartRowIndexParameterName = "startRow";
            this.MaximumRowsParameterName = "maxRows";

            // 生成一个分页设配器.
            this.m_pagingAdapter = new GridViewPagingAdapter(dataSource, virtualItemCount);

            // 关键在这里 - 把已构造好的业分页设配器替换掉自身的数据源。
            this.ObjectCreating += new ObjectDataSourceObjectEventHandler(ObjectDataSourceExtension_ObjectCreating);
        }

        void ObjectDataSourceExtension_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this.m_pagingAdapter;
        }
    }
    /// <summary>
    /// 分页适配器。
    /// </summary>
    internal class GridViewPagingAdapter
    {
        private DataTable  m_dataList;
        private int m_virtualItemCount;

        // 构造一个适配器。
        public GridViewPagingAdapter(DataTable list, int rowCount)
        {
            m_dataList = list;
            m_virtualItemCount = rowCount;
        }

        // 取全部的数据（其实也只是一页的数据啦）。
        public DataTable GetData()
        {
            return m_dataList;
        }

        // 返回你设定的记录数。
        public int VirtualItemCount()
        {
            return m_virtualItemCount;
        }
        // 取一页的数据。
        public DataTable GetData(int startRow, int maxRows)
        {
            return m_dataList;
        }
    }

}
