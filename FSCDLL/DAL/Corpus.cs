using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSCDLL.DAL
{
    public class Corpus
    {
        #region 读取
        /// <summary>
        /// 获取所有语料库的内容
        /// </summary>
        /// <returns></returns>
        public static DataSet GetCorpus()
        {
            DataSet ds = SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "FSC_GetCorpus");
            return ds;
        }

        /// <summary>
        /// 查询指定ID的语料
        /// </summary>
        /// <param name="corporaId"></param>
        /// <returns></returns>
        public static DataSet GetCorporaByID(long corporaId)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetCorporaByID", corporaId);
            return ds;
        }

        /// <summary>
        /// 获取所有学术语料库的年份，YEAE
        /// </summary>
        /// <returns></returns>
        public static DataSet GetCorpusYearsOfAC()
        {
            DataSet ds = SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetCorpusYearsOfAC");
            return ds;
        }
        /// <summary>
        /// 通过查询条件获取所有语料库的内容
        /// </summary>
        /// <param name="filterExpression">由字符串组成的查询条件</param>
        /// <returns></returns>
        public static DataSet GetCorpusByFilterString(string filterExpression)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetCorpusBySql", filterExpression);
            return ds;
        }
        /// <summary>
        /// 获取语法表中的常量表数据
        /// </summary>
        /// <param name="types">类型</param>
        /// <returns></returns>
        public static DataSet GetCopusExtendByTypes(string types)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetCorpusExtendByTypes", types);
            return ds;
        }
        /// <summary>
        /// 获取学术语料库中的专业,绑定TitleCN,Title
        /// </summary>
        /// <param name="majorID">四位专业编号</param>
        /// <returns></returns>
        public static DataSet GetCorpusMajor()
        {
            DataSet ds = DAL.Corpus.GetCopusExtendByTypes("Major");
            return ds;
        }
        /// <summary>
        /// 获取学术语料库中期刊来源
        /// </summary>
        /// <param name="majorID">四位专业编号</param>
        /// <returns></returns>
        public static DataSet GetCorpusJournalByMajor(string majorID)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetCorpusJournalByMajor", majorID);
            return ds;
        }
        #endregion
        #region 更新
        /// <summary>
        /// 更新语料的行
        /// </summary>
        /// <param name="trans">事务，如果只是一条，则传值null</param>
        /// <param name="dr">包含语料的要更新数据行</param>
        /// <returns></returns>
        public static int UpdateCorpus(SqlTransaction trans, DataRow dr)
        {
            if (trans == null)
                return (DAL.SqlHelper.ExecuteAppointedParameters(DAL.DataProvider.ConnectionString, "FSC_UpdateCorpus", dr));
            else
                return (DAL.SqlHelper.ExecuteAppointedParameters(trans, "FSC_UpdateCorpus", dr));
        }
        #endregion
        #region 添加
        /// <summary>
        /// 添加新的语料，Source默认为LC
        /// </summary>
        /// <param name="trans">事务，如果只是一条，则传值null</param>
        /// <param name="source">语料库的来源LC、AC</param>
        /// 
        /// <returns></returns>
        public static long InsertCorpus(SqlTransaction trans, DataRow dr,string source="LC")
        {
            dr["Source"] = source;
            if (trans == null)

                return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(DAL.DataProvider.ConnectionString, "FSC_InsertCorpus", dr)[0].Value);
            else
                return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(trans, "FSC_InsertCorpus", dr)[0].Value);
        }


        #endregion

    }
}
