using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace FSCWebParts.Corpus
{
    /// <summary>
    /// 语料库通过文本文件导入导出
    /// </summary>
    public class CorpusProcessing
    {

        public void SubFillGridView(GridView grid)
        {
            //得到数据和构造一个VirtualObjectDataSource.
            DataTable list = GetDataSource(grid.PageIndex);
            int rowCount = GetRowCount();
            VirtualObjectDataSource ods = new FSCWebParts.VirtualObjectDataSource(list, rowCount);
            // 数据绑定.
            grid.DataSource = ods;
            grid.DataBind();
            //完成!
        }
        int GetRowCount()
        {
            return 0;
        }
        DataTable GetDataSource(int pageIndex)
        {
            DataTable dt = null;
            return dt;
        }
    }
}
